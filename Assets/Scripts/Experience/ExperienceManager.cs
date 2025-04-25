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
            7000,   // 6
            10000,  // 7
            15000,  // 8
            20000,  // 9
            25000,  // 10
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