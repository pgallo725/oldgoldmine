using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    class GameOverMenu : Menu
    {
        private SpriteText menuTitle;

        private Button replayButton;
        private Button menuButton;

        private SpriteText scoreText;
        private SpriteText newHighscoreText;

        public override void Initialize(Viewport viewport, Texture2D background, Menu parent = null)
        {
            this.parent = parent;
            this.menuBackground = background;
            this.semiTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.33f)), viewport.Bounds.Center.ToVector2(), viewport.Bounds.Size.ToVector2());

            Vector2 buttonSize = new Vector2(400, 120);


            // GAMEOVER MENU LAYOUT SETUP

            menuTitle = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "YOU DIED",
                new Color(160, 0, 0, 255), new Vector2(viewport.Width / 2, viewport.Height * 0.1f), SpriteText.TextAnchor.MiddleCenter);

            replayButton = new Button(viewport.Bounds.Center.ToVector2() - new Vector2(0, 50), buttonSize, 
                OldGoldMineGame.resources.menuItemsFont, "PLAY AGAIN", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            menuButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 100), buttonSize, 
                OldGoldMineGame.resources.menuItemsFont, "BACK TO MENU", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            scoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Final score: " + OldGoldMineGame.Score,
                Color.LightGoldenrodYellow, new Vector2(viewport.Width / 2, (viewport.Height - buttonSize.Y) / 2 - 175),
                SpriteText.TextAnchor.MiddleCenter);

            newHighscoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "NEW HIGHSCORE!",
                Color.Orange, scoreText.Position + new Vector2(0, 55), SpriteText.TextAnchor.MiddleCenter);
        }


        public override void Update()
        {
            if (replayButton.Update() || InputManager.PauseKeyPressed)
            {
                OldGoldMineGame.Application.RestartGame();
            }
            else if (menuButton.Update())
            {
                OldGoldMineGame.Application.ToMainMenu();
            }
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.Black);

            spriteBatch.Begin();

            if (menuBackground != null)
            {
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);
                semiTransparencyLayer.Draw(spriteBatch);
            }

            menuTitle.Draw(spriteBatch);
            replayButton.Draw(spriteBatch);
            menuButton.Draw(spriteBatch);
            scoreText.Draw(spriteBatch);
            newHighscoreText.Draw(spriteBatch);

            spriteBatch.End();
        }


        public override void Show()
        {
            scoreText.Text = "Final score: " + OldGoldMineGame.Score;
            newHighscoreText.Enabled = OldGoldMineGame.Score > OldGoldMineGame.BestScore;

            replayButton.Enabled = true;
            menuButton.Enabled = true;
        }


        public override void CloseSubmenu()
        {
            // GameOverMenu has no nested submenus
            throw new System.NotSupportedException();
        }
    }
}
