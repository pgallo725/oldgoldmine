using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.UI;


namespace OldGoldMine.Gameplay
{
    public class HUD
    {
        public static HUD Instance;

        private readonly SpriteText timerText;
        private readonly SpriteText framerateText;
        private readonly SpriteText scoreText;
        private readonly SpriteText speedText;

        private bool framerateVisible = false;
        private Rectangle area;


        /// <summary>
        /// Create a singleton instance of the HUD class, accessible via the static Instance field.
        /// </summary>
        /// <param name="window">The application window in which the HUD will be drawn.</param>
        public static void Create(in GameWindow window)
        {
            Instance = new HUD(window);
        }

        private HUD(in GameWindow window)
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


        /// <summary>
        /// Update the timer shown in the HUD with the value of the provided object.
        /// </summary>
        /// <param name="timer">Timer object that will be used to update the HUD timer.</param>
        public void UpdateTimer(Timer timer)
        {
            timerText.Text = timer.ToString();
        }

        /// <summary>
        /// Update the framerate counter shown in the HUD with the provided value.
        /// </summary>
        /// <param name="framerate">Framerate to be shown on the HUD (if enabled), in FPS.</param>
        public void UpdateFramerate(double framerate)
        {
            if (framerateVisible)
            {
                framerateText.Text = framerate.ToString("0.# FPS");
                framerateText.Color = framerate < 60f ? Color.Red : Color.LimeGreen;
            }
        }

        /// <summary>
        /// Toggle the visibility of the framerate counter in the HUD.
        /// </summary>
        public void ToggleFramerateVisible()
        {
            framerateVisible = !framerateVisible;
        }

        /// <summary>
        /// Update the score label of the HUD with the provided value.
        /// </summary>
        /// <param name="score">Score value to be shown on the HUD, in points.</param>
        public void UpdateScore(int score)
        {
            scoreText.Text = score.ToString("Score: 0.#");
        }

        /// <summary>
        /// Update the speed indicator of the HUD with the provided value.
        /// </summary>
        /// <param name="speed">Speed value to be shown on the HUD, in Km/h.</param>
        public void UpdateSpeed(float speed)
        {
            speedText.Text = speed.ToString("Speed: 0.# Km/h");
        }


        /// <summary>
        /// Prepares the HUD to be shown on screen, laying out its elements in the available window.
        /// </summary>
        /// <param name="window">The application window in which the HUD is drawn.</param>
        public void Show(in GameWindow window)
        {
            // Update the HUD layout if the window area has been resized
            if (window.ClientBounds != area)
            {
                area = window.ClientBounds;
                Layout();
            }
        }

        private void Layout()
        {
            Viewport viewport = OldGoldMineGame.graphics.GraphicsDevice.Viewport;

            timerText.Position = new Point(viewport.Bounds.Width / 2, 5);
            framerateText.Position = new Point(viewport.Bounds.Width - 10, 5);
            scoreText.Position = new Point(15, 5);
            speedText.Position = new Point(15, 50);
        }


        /// <summary>
        /// Draw the HUD on the screen.
        /// </summary>
        /// <param name="spriteBatch">A SpriteBatch object that will be used to draw the menu elements.
        /// It will Begin() and End() inside this call.</param>
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
