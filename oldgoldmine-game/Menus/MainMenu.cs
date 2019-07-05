using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    class MainMenu : Menu
    {

        private Button playButton;
        private Button exitButton;

        private SpriteText highscoreText;


        public override void Initialize(Viewport viewport, Texture2D background)
        {
            this.menuBackground = background;

            Vector2 buttonSize = new Vector2(400, 120);


            // MAIN MENU LAYOUT SETUP

            playButton = new Button(viewport.Bounds.Center.ToVector2() - new Vector2(0, 75), buttonSize, 
                OldGoldMineGame.resources.menuButtonFont, "PLAY", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);

            exitButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 75), buttonSize,
                OldGoldMineGame.resources.menuButtonFont, "QUIT", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);


            highscoreText = new SpriteText(OldGoldMineGame.resources.menuButtonFont, "Highscore: " + OldGoldMineGame.BestScore,
                Color.LightGoldenrodYellow, new Vector2(viewport.Width / 2, (viewport.Height + buttonSize.Y) / 2 + 160),
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
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);

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
