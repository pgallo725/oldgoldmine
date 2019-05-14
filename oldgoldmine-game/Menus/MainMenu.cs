using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace oldgoldmine_game.Menus
{
    class MainMenu : Menu
    {
        private readonly Vector2 buttonSize = new Vector2(400, 120);
        private const int elementSeparation = 70;

        private Rectangle playButtonRectangle;
        private Rectangle exitButtonRectangle;

        private Vector2 playTextPosition;
        private Vector2 exitTextPosition;
        private Vector2 highscoreTextPosition;

        private bool playButtonHighlighted = false;
        private bool exitButtonHighlighted = false;

        private MouseState lastMouseState;
        private MouseState currentMouseState;


        public override void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton)
        {
            this.menuBackground = background;
            this.menuFont = font;
            this.menuSpriteBatch = new SpriteBatch(device);
            this.buttonTextureNormal = normalButton;
            this.buttonTextureHighlighted = highlightedButton;


            // Main menu layout setup
            playButtonRectangle = new Rectangle(device.Viewport.Width/2 - (int)buttonSize.X/2,
                device.Viewport.Height/2 - (int)buttonSize.Y/2 - elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            exitButtonRectangle = new Rectangle(device.Viewport.Width/2 - (int)buttonSize.X/2,
                device.Viewport.Height/2 - (int)buttonSize.Y/2 + elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            playTextPosition = playButtonRectangle.Center.ToVector2()
                - (menuFont.MeasureString("PLAY") / 2);

            exitTextPosition = exitButtonRectangle.Center.ToVector2()
                - (menuFont.MeasureString("QUIT") / 2);

            highscoreTextPosition = new Vector2(device.Viewport.Width/2,
                exitButtonRectangle.Bottom + (elementSeparation / 2));


            lastMouseState = Mouse.GetState();
        }


        public override void Update(OldGoldMineGame application)
        {
            currentMouseState = Mouse.GetState();

            // Check if the mouse cursor is above one of the menu buttons
            playButtonHighlighted = playButtonRectangle.Contains(currentMouseState.Position);
            exitButtonHighlighted = exitButtonRectangle.Contains(currentMouseState.Position);

            // Recognize a single click of the left mouse button
            if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                // React to the click
                Rectangle clickArea = new Rectangle(currentMouseState.Position, new Point(10, 10));

                if (clickArea.Intersects(playButtonRectangle))
                    application.StartGame();
                else if (clickArea.Intersects(exitButtonRectangle))
                    application.Exit();
            }

            lastMouseState = currentMouseState;
        }


        public override void Draw(GraphicsDevice screen)
        {
            screen.Clear(Color.Black);

            menuSpriteBatch.Begin();

            menuSpriteBatch.Draw(playButtonHighlighted ? buttonTextureHighlighted : buttonTextureNormal,
                destinationRectangle: playButtonRectangle, Color.BurlyWood);
            menuSpriteBatch.DrawString(menuFont, "PLAY", playTextPosition, Color.White);

            menuSpriteBatch.Draw(exitButtonHighlighted ? buttonTextureHighlighted : buttonTextureNormal,
                destinationRectangle: exitButtonRectangle, Color.BurlyWood);
            menuSpriteBatch.DrawString(menuFont, "QUIT", exitTextPosition, Color.White);

            int testScore = 17520;
            string highscoreText = testScore.ToString("Highscore: 0.#");

            menuSpriteBatch.DrawString(menuFont, highscoreText, highscoreTextPosition 
                - menuFont.MeasureString(highscoreText) / 2, Color.LightGoldenrodYellow);

            menuSpriteBatch.End();
        }

    }
}
