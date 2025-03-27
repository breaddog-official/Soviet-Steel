using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.SplitScreen
{
    public static class SplitScreenManager
    {
        public static readonly Dictionary<long, SplitScreenPlayer> players = new();


        public static long AddPlayer(string name, Rect viewport, InputDevice device)
        {
            long id = RandomLong();
            players.Add(id, new SplitScreenPlayer(name, viewport, device));

            return id;
        }

        public static bool RemovePlayer(long id)
        {
            return players.Remove(id);
        }



        public static IReadOnlyDictionary<long, SplitScreenPlayer> GetPlayers() => players;


        public static long RandomLong()
        {
            int value1 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            int value2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            return value1 + ((long)value2 << 32);
        }
    }

    [Serializable]
    public class SplitScreenPlayer
    {
        public string name;
        public Rect viewport = new(Vector2.zero, Vector2.one);

        //[HideInInspector] public long id;
        [HideInInspector] public InputDevice device;



        public SplitScreenPlayer(string name, Rect viewport, InputDevice device/*, long id*/)
        {
            this.name = name;
            this.viewport = viewport;
            this.device = device;
            //this.id = id;
        }
    }
}
