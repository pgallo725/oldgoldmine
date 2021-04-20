using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.UI;


namespace OldGoldMine.Gameplay
{
    public class HUD
    {
        private readonly SpriteText timerText;
        private readonly SpriteText framerateText;
        private readonly SpriteText scoreText;
        private readonly SpriteText speedText;

        private bool framerateVisible = false;
        private Rectangle area;


        public HUD(in GameWindow window)
        {
            this.area = window.ClientBounds;

            // SETUP HUD ELEMENTS

            timerText = new SpriteText(OldGoldMineGame.resources.hudFont, "00:00:00", 
                Color.White, new Point(window.ClientBounds.Width / 2, 5), SpriteText.TextAnchor.TopCenter);

            framerateText = new SpriteText(OldGoldMineGame.resources.debugInfoFont, "0 FPS",
                Color.LightGreen, new Point(window.ClientBounds.Width - 10, 5), SpriteText.TextAnchor.TopRight);

            scoreText = new SpriteText(OldGoldMineGame.resources.hudFont, "Score: 0",
                Color.White, new Point(15, 5), SpriteText.TextAnchor.TopLeft);

            speedText = new SpriteText(OldGoldMineGame.resources.hudFont, "Speed: 20 Km/h",
                Color.White, new Point(15, 50), SpriteText.TextAnchor.TopLeft);
        }


        public void UpdateTimer(Timer timer)
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


        private void Layout()
        {
            Viewport viewport = OldGoldMineGame.graphics.GraphicsDevice.Viewport;

            timerText.Position = new Point(viewport.Bounds.Width / 2, 5);
            framerateText.Position = new Point(viewport.Bounds.Width - 10, 5);
            scoreText.Position = new Point(15, 5);
            speedText.Position = new Point(15, 50);
        }

        public void Show(in GameWindow window)
        {
            // Update the HUD layout if the window area has been resized
            if (window.ClientBounds != area)
            {
                area = window.ClientBounds;
                Layout();
            }
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
