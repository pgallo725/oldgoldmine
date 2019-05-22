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


        public override void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton)
        {
            this.menuBackground = background;


            // Main menu layout setup
            Rectangle playButtonRectangle = new Rectangle(device.Viewport.Width/2 - (int)buttonSize.X/2,
                device.Viewport.Height/2 - (int)buttonSize.Y/2 - elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            Rectangle exitButtonRectangle = new Rectangle(device.Viewport.Width/2 - (int)buttonSize.X/2,
                device.Viewport.Height/2 - (int)buttonSize.Y/2 + elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            Vector2 highscoreTextPosition = new Vector2(device.Viewport.Width/2,
                exitButtonRectangle.Bottom + (elementSeparation / 2));


            playButton = new Button(playButtonRectangle, font, "PLAY", normalButton, highlightedButton);
            exitButton = new Button(exitButtonRectangle, font, "QUIT", normalButton, highlightedButton);

            highscoreText = new SpriteText(font, "Highscore: 17520", Color.LightGoldenrodYellow,
                highscoreTextPosition, SpriteText.TextAlignment.MiddleCenter);
        }


        public override void Update(in OldGoldMineGame application)
        {
            playButton.Update();
            exitButton.Update();

            if (playButton.IsClicked())
                application.StartGame();
            else if (exitButton.IsClicked())
                application.Exit();
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

    }
}
