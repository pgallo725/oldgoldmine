using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.Gameplay;
using OldGoldMine.UI;

namespace OldGoldMine.Menus
{
    class GameOverMenu : Menu
    {
        private readonly SpriteText menuTitle;

        private readonly Button replayButton;
        private readonly Button menuButton;

        private readonly SpriteText scoreText;
        private readonly SpriteText newHighscoreText;


        public GameOverMenu(Viewport viewport, Texture2D background, Menu parent = null)
            : base(background, new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.33f)), new Point(400, 120), parent)
        {
            // GAMEOVER MENU LAYOUT SETUP

            menuTitle = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "YOU DIED",
                new Color(160, 0, 0, 255), new Point(viewport.Width / 2, viewport.Height / 10));

            replayButton = new Button(viewport.Bounds.Center - new Point(0, 50), buttonSize, 
                OldGoldMineGame.resources.menuItemsFont, "PLAY AGAIN", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            menuButton = new Button(viewport.Bounds.Center + new Point(0, 100), buttonSize, 
                OldGoldMineGame.resources.menuItemsFont, "BACK TO MENU", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            scoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Final score: " + Score.Current,
                Color.LightGoldenrodYellow, new Point(viewport.Width / 2, (viewport.Height - buttonSize.Y) / 2 - 175));

            newHighscoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "NEW HIGHSCORE!",
                Color.Orange, scoreText.Position + new Point(0, 55));
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.graphics.GraphicsDevice.Viewport;
            Point center = viewport.Bounds.Center;

            menuTitle.Position = new Point(viewport.Width / 2, viewport.Height / 10);

            replayButton.Position = center - new Point(0, 50);
            menuButton.Position = center + new Point(0, 100);

            scoreText.Position = new Point(viewport.Width / 2, (viewport.Height - buttonSize.Y) / 2 - 175);
            newHighscoreText.Position = scoreText.Position + new Point(0, 55);
        }


        public override void Show()
        {
            Layout();

            scoreText.Text = "Final score: " + Score.Current;
            newHighscoreText.Enabled = Score.Current > Score.Best;

            replayButton.Enabled = true;
            menuButton.Enabled = true;
        }


        public override void Update()
        {
            if (replayButton.Update())
            {
                OldGoldMineGame.Application.RestartGame();
            }
            else if (menuButton.Update() || InputManager.PausePressed)
            {
                OldGoldMineGame.Application.ToMainMenu();
            }
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.Black);

            spriteBatch.Begin();

            if (background != null)
            {
                spriteBatch.Draw(background, screen.Viewport.Bounds, Color.White);
                spriteBatch.Draw(middleLayer, screen.Viewport.Bounds, Color.White);
            }

            menuTitle.Draw(spriteBatch);
            replayButton.Draw(spriteBatch);
            menuButton.Draw(spriteBatch);
            scoreText.Draw(spriteBatch);
            newHighscoreText.Draw(spriteBatch);

            spriteBatch.End();
        }

    }
}
