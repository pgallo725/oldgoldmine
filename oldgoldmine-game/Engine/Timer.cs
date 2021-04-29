using System;
using Microsoft.Xna.Framework;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Simple class useful for holding a time value that can be easily
    /// set, reset and updated inside the game loop.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// The TimeSpan measured by the Timer since it was last reset.
        /// </summary>
        public TimeSpan Time = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Set the Timer value to the specified number of hours, minutes and seconds.
        /// </summary>
        /// <param name="hours">Amount of hours for the Timer value.</param>
        /// <param name="minutes">Amount of minutes for the Timer value.</param>
        /// <param name="seconds">Amount of seconds for the Timer value.</param>
        public void Set(int hours, int minutes, int seconds)
        {
            Time = new TimeSpan(hours, minutes, seconds);
        }

        /// <summary>
        /// Reset the Timer to 0 hours, 0 minutes, 0 seconds.
        /// </summary>
        public void Reset()
        {
            Time = new TimeSpan(0, 0, 0);
        }

        /// <summary>
        /// Tick the Timer in the current frame, updating its value accordingly.
        /// </summary>
        /// <param name="elapsedTime">Time signature of the current frame.</param>
        public void Update(GameTime elapsedTime)
        {
            Time = Time.Add(elapsedTime.ElapsedGameTime);
        }

        /// <summary>
        /// Format the Timer value to a string.
        /// </summary>
        /// <returns>String representation of the Timer value (in the format hh:mm:ss).</returns>
        public override string ToString()
        {
            return Time.ToString("hh\\:mm\\:ss");
        }

    }
}
