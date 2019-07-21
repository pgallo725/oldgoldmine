using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;


namespace oldgoldmine_game.Menus
{
    class MainMenu : Menu
    {
        private SpriteText gameTitle;

        private Button playButton;
        private Button exitButton;

        private SpriteText highscoreText;


        public override void Initialize(Viewport viewport, Texture2D background)
        {
            this.menuBackground = background;
            this.semiTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.5f)), viewport.Bounds.Center.ToVector2(), viewport.Bounds.Size.ToVector2());

            Vector2 buttonSize = new Vector2(400, 120);


            // MAIN MENU LAYOUT SETUP

            gameTitle = new SpriteText(OldGoldMineGame.resources.gameTitleFont, "Old Gold Mine",
                Color.DarkGoldenrod, new Vector2(viewport.Width / 2, viewport.Height * 0.15f), SpriteText.TextAnchor.MiddleCenter);

            playButton = new Button(viewport.Bounds.Center.ToVector2() - new Vector2(0, 75), buttonSize, 
                OldGoldMineGame.resources.menuItemsFont, "PLAY", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures);

            exitButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 75), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "QUIT", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures);


            highscoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Highscore: " + OldGoldMineGame.BestScore,
                new Color(120, 210, 50, 255), new Vector2(viewport.Width / 2, (viewport.Height + buttonSize.Y) / 2 + 160),
                SpriteText.TextAnchor.MiddleCenter);
        }


        public override void Update()
        {
            playButton.Update();
            exitButton.Update();

            if (playButton.IsClicked())
                OldGoldMineGame.Application.NewGame();
            else if (exitButton.IsClicked())
                OldGoldMineGame.Application.Exit();
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

            gameTitle.Draw(spriteBatch);

            playButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);

            highscoreText.Draw(spriteBatch);

            spriteBatch.End();
        }


        public override void Show()
        {
            highscoreText.Text = "Highscore: " + OldGoldMineGame.BestScore;

            playButton.Enabled = true;
            exitButton.Enabled = true;
        }
    }
}
