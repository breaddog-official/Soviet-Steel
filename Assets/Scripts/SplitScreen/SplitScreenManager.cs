using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.SplitScreen
{
    public static class SplitScreenManager
    {
        public static readonly List<SplitScreenPlayer> players = new();


        public static int AddPlayer(string name, SplitScreenDevice device)
        {
            players.Add(new SplitScreenPlayer
            {
                name = name,
                device = device
            });

            RebuildRects();

            return players.Count - 1;
        }

        public static void RemovePlayer(int playerIndex)
        {
            players.RemoveAt(playerIndex);
        }

        public static void RemoveAllPlayers()
        {
            players.Clear();
        }


        public static int GetPlayerIndex(SplitScreenPlayer player)
        {
            return players.IndexOf(player);
        }


        #region Rects

        // Я знаю, это ужас
        private static readonly Rect[] onePlayersRect = new Rect[1]
        {
            new(0, 0, 1, 1f),
        };

        private static readonly Rect[] twoPlayersRect = new Rect[2]
        {
            new(0, 0, 0.5f, 1f),
            new(0.5f, 0, 0.5f, 1f),
        };

        private static readonly Rect[] threePlayersRect = new Rect[3]
        {
            new(0, 0, 1f, 0.5f),
            new(0, 0.5f, 0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f, 0.5f),
        };

        private static readonly Rect[] fourPlayersRect = new Rect[4]
        {
            new(0, 0.5f, 0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f, 0.5f),
            new(0, 0, 0.5f, 0.5f),
            new(0.5f, 0, 0.5f, 0.5f),
        };

        private static void RebuildRects()
        {
            Rect[] rects = players.Count switch
            {
                1 => onePlayersRect,
                2 => twoPlayersRect,
                3 => threePlayersRect,
                4 => fourPlayersRect,
                0 => throw new Exception("Minimum 1 split screen player!"),
                _ => throw new Exception("Maximum 4 split screen players!")
            };

            for (int i = 0; i < players.Count; i++)
            {
                players[i].viewport = rects[i];
            }
        }

        #endregion


        public static IReadOnlyList<SplitScreenPlayer> GetPlayers() => players;
    }

    public enum SplitScreenDevice
    {
        None,
        KeyboardLeft,
        KeyboardRight,
        Gamepad,
        Joystick
    }

    [Serializable]
    public class SplitScreenPlayer
    {
        public string name;
        public string carHash;
        public SplitScreenDevice device;
        public Rect viewport;
    }
}
