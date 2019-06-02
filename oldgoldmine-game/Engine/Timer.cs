using System;
using Microsoft.Xna.Framework;

namespace oldgoldmine_game.Engine
{
    public class Timer
    {

        public TimeSpan time = new TimeSpan(0, 0, 0);

        public void Reset()
        {
            time = new TimeSpan(0, 0, 0);
        }

        public void Update(GameTime elapsedTime)
        {
            time = time.Add(elapsedTime.ElapsedGameTime);
        }

        public override string ToString()
        {
            return time.Hours.ToString("00") +
                ":" + time.Minutes.ToString("00") +
                ":" + time.Seconds.ToString("00");
        }

    }
}
