using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.UI;

namespace OldGoldMine.Gameplay
{
    public class HUD
    {

        SpriteText timerText;
        SpriteText framerateText;
        SpriteText scoreText;
        SpriteText speedText;

        private bool framerateVisible = false;


        public void Initialize(in GameWindow window)
        {
            // HUD elements setup
            timerText = new SpriteText(OldGoldMineGame.resources.hudFont, "00:00:00", Color.White,
                new Vector2(window.ClientBounds.Width / 2, 5), SpriteText.TextAnchor.TopCenter);

            framerateText = new SpriteText(OldGoldMineGame.resources.debugInfoFont, "0 FPS", Color.LightGreen,
                new Vector2(window.ClientBounds.Width - 10, 5), SpriteText.TextAnchor.TopRight);

            scoreText = new SpriteText(OldGoldMineGame.resources.hudFont, "Score: 0", Color.White,
                new Vector2(15, 5), SpriteText.TextAnchor.TopLeft);

            speedText = new SpriteText(OldGoldMineGame.resources.hudFont, "Speed: 20 Km/h", Color.White,
                new Vector2(15, 50), SpriteText.TextAnchor.TopLeft);
        }


        public void UpdateTimer(Engine.Timer timer)
        {
            timerText.Text = timer.ToString();
        }

        public void UpdateFramerate(double framerate)
        {
            if (framerateVisible)
            {
                framerateText.Text = framerate.ToString("0.# FPS");
                framerateText.Color = framerate < 60f ? Color.Red : Color.LimeGreen;
            }
        }

        public void ToggleFramerateVisible()
        {
            framerateVisible = !framerateVisible;
        }

        public void UpdateScore(int score)
        {
            scoreText.Text = score.ToString("Score: 0.#");
        }

        public void UpdateSpeed(float speed)
        {
            speedText.Text = speed.ToString("Speed: 0.# Km/h");
        }


        public void Show(Engine.Timer time, double framerate, int score, float speed)
        {
            UpdateTimer(time);
            UpdateFramerate(framerate);
            UpdateScore(score);
            UpdateSpeed(speed);
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            timerText.Draw(in spriteBatch);            // Show timer
            if (framerateVisible)
                framerateText.Draw(in spriteBatch);    // Show framerate
            scoreText.Draw(in spriteBatch);            // Show score
            speedText.Draw(in spriteBatch);            // Show speed

            spriteBatch.End();
        }
    }
}
