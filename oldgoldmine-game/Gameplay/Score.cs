using System;
using Microsoft.Win32;

namespace OldGoldMine.Gameplay
{
    public static class Score
    {
        const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game";

        public static float Multiplier { get; set; } = 1f;
        public static int Current { get; set; } = 0;
        public static int Best { get; private set; } = 0;


        /// <summary>
        /// Update the current score by adding the specified amount of points.
        /// </summary>
        /// <param name="points">The amount of points that have to be added to the current score.</param>
        public static void Add(uint points)
        {
            Current += (int)(points * Multiplier + 0.5f);    // extra 0.5f added to avoid int conversion errors

            HUD.Instance.UpdateScore(Current);
        }

        /// <summary>
        /// Update the highscore and save it to the Windows registry, to keep it across multiple runs.
        /// </summary>
        public static void Save()
        {
            if (Current > Best)
            {
                Best = Current;
                Registry.SetValue(key, "Highscore", Best);
            }
        }

        /// <summary>
        /// Load the user's previous best score from the Windows registry.
        /// </summary>
        /// <returns>The highscore for the current user (or 0 if no previous score is found).</returns>
        public static int Load()
        {
            try
            {
                int? score = Registry.GetValue(key, "Highscore", 0) as int?;

                // type int? is nullable (if key doesn't exist)
                Best = score ?? 0;
            }
            catch (Exception)
            {
                Best = 0;
            }

            return Best;
        }
    }
}
