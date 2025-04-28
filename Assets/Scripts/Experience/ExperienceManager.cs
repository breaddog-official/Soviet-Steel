using System;

namespace Scripts.Gameplay.Experience
{
    public static class ExperienceManager
    {
        public static uint Experience { get; private set; }

        /// <summary>
        /// Old Experience and New Experience
        /// </summary>
        public static event Action<uint, uint> OnExperienceChanged;

        public static readonly uint[] levels = new uint[]
        {
            0,      // 1
            1000,   // 2
            2000,   // 3
            3500,   // 4
            5000,   // 5
            6500,   // 6
            8000,   // 7
            10_000,  // 8
            12_500,  // 9
            15_000,  // 10
            17_500,  // 11
            20_000,  // 12 - EndGame
            25_000,  // 13
            30_000,  // 14
            35_000,  // 15
            40_000,  // 16
            45_000,  // 17
            50_000,  // 18
            55_000,  // 19
            60_000,  // 20
            65_000,  // 21
            70_000,  // 23
            80_000,  // 22
            90_000,  // 24
            100_000,  // 25
            150_000,  // 26
            200_000,  // 27
            300_000,  // 28
            400_000,  // 29
            500_000,  // 30
        };


        public static void SetExperience(uint value)
        {
            Experience = value;
            OnExperienceChanged?.Invoke(Experience, Experience);
        }

        public static void EncreaseExperience(uint value)
        {
            Experience += value;
            OnExperienceChanged?.Invoke(Experience - value, Experience);
        }

        public static void DecreaseExperience(uint value)
        {
            if (value > Experience)
                value = Experience;

            Experience -= value;
            OnExperienceChanged?.Invoke(Experience + value, Experience);
        }


        public static int GetPreviousLevel()
        {
            var level = GetCurrentLevel();
            if (level <= 1)
                return 1;
            else
                return level - 1;
        }

        public static int GetNextLevel()
        {
            var level = GetCurrentLevel();
            if (level >= levels.Length)
                return levels.Length;
            else
                return level + 1;
        }

        public static int GetCurrentLevel()
        {
            return GetLevel(Experience);
        }

        public static int GetLevel(uint experience)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                if (experience < levels[i])
                    return i;
            }
            return levels.Length;
        }
    }
}