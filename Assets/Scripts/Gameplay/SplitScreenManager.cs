using System;
using System.Collections.Generic;
using Scripts.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.SplitScreen
{
    public static class SplitScreenManager
    {
        public static readonly Dictionary<long, SplitScreenPlayer> players = new();


        public static long AddPlayer(string name, Rect viewport, InputDevice device)
        {
            long id = RandomE.RandomLong();
            players.Add(id, new SplitScreenPlayer(name, viewport, device));

            return id;
        }

        public static bool RemovePlayer(long id)
        {
            return players.Remove(id);
        }



        public static IReadOnlyDictionary<long, SplitScreenPlayer> GetPlayers() => players;
    }

    [Serializable]
    public class SplitScreenDevice
    {
        public string controlScheme;
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
