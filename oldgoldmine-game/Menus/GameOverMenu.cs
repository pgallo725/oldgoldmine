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

        public override void Initialize(Viewport viewport, Texture2D background)
        {
            this.menuBackground = background;

            Vector2 buttonSize = new Vector2(400, 120);


            // GAMEOVER MENU LAYOUT SETUP

            menuTitle = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "YOU DIED",
                Color.White, new Vector2(viewport.Width / 2, viewport.Height * 0.075f), SpriteText.TextAnchor.MiddleCenter);

            replayButton = new Button(viewport.Bounds.Center.ToVector2() - new Vector2(0, 50), buttonSize, 
                OldGoldMineGame.resources.menuButtonFont, "PLAY AGAIN", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);

            menuButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 100), buttonSize, 
                OldGoldMineGame.resources.menuButtonFont, "BACK TO MENU", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);

            scoreText = new SpriteText(OldGoldMineGame.resources.menuButtonFont, "Final score: " + OldGoldMineGame.Score,
                Color.LightGoldenrodYellow, new Vector2(viewport.Width / 2, (viewport.Height - buttonSize.Y) / 2 - 175),
                SpriteText.TextAnchor.MiddleCenter);

            newHighscoreText = new SpriteText(OldGoldMineGame.resources.menuButtonFont, "NEW HIGHSCORE!",
                Color.Orange, scoreText.Position + new Vector2(0, 55), SpriteText.TextAnchor.MiddleCenter);
        }


        public override void Update()
        {
            replayButton.Update();
            menuButton.Update();

            if (InputManager.PauseKeyPressed)
                OldGoldMineGame.Application.ResumeGame();

            if (replayButton.IsClicked())
                OldGoldMineGame.Application.RestartGame();
            else if (menuButton.IsClicked())
                OldGoldMineGame.Application.ToMainMenu();
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.DarkRed);

            spriteBatch.Begin();

            if (menuBackground != null)
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);

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
    }
}
