using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace oldgoldmine_game.Menus
{
    class PauseMenu : Menu
    {
        private readonly Vector2 buttonSize = new Vector2(400, 120);
        private const int elementSeparation = 70;

        private Rectangle resumeButtonRectangle;
        private Rectangle menuButtonRectangle;

        private Vector2 resumeTextPosition;
        private Vector2 menuTextPosition;

        private bool resumeButtonHighlighted = false;
        private bool menuButtonHighlighted = false;


        public override void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton)
        {
            this.menuBackground = background;
            this.menuFont = font;
            this.menuSpriteBatch = new SpriteBatch(device);
            this.buttonTextureNormal = normalButton;
            this.buttonTextureHighlighted = highlightedButton;


            // Pause menu layout setup
            resumeButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 - elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            menuButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 + elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            resumeTextPosition = resumeButtonRectangle.Center.ToVector2() 
                - (menuFont.MeasureString("RESUME") / 2);

            menuTextPosition = menuButtonRectangle.Center.ToVector2()
                - (menuFont.MeasureString("BACK TO MENU") / 2);


            lastMouseState = Mouse.GetState();
        }


        public override void Update(OldGoldMineGame application)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                application.ResumeGame();

            currentMouseState = Mouse.GetState();

            // Check if the mouse cursor is above one of the menu buttons
            resumeButtonHighlighted = resumeButtonRectangle.Contains(currentMouseState.Position);
            menuButtonHighlighted = menuButtonRectangle.Contains(currentMouseState.Position);

            // Recognize a single click of the left mouse button
            if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                // React to the click
                Rectangle clickArea = new Rectangle(currentMouseState.Position, new Point(10, 10));

                if (clickArea.Intersects(resumeButtonRectangle))
                    application.StartGame();
                else if (clickArea.Intersects(menuButtonRectangle))
                    application.ToMainMenu();
            }

            lastMouseState = currentMouseState;
        }


        public override void Draw(GraphicsDevice screen)
        {
            screen.Clear(Color.DarkOrange);

            menuSpriteBatch.Begin();

            menuSpriteBatch.Draw(resumeButtonHighlighted ? buttonTextureHighlighted : buttonTextureNormal,
                destinationRectangle: resumeButtonRectangle, Color.BurlyWood);
            menuSpriteBatch.DrawString(menuFont, "RESUME", resumeTextPosition, Color.White);

            menuSpriteBatch.Draw(menuButtonHighlighted ? buttonTextureHighlighted : buttonTextureNormal,
                destinationRectangle: menuButtonRectangle, Color.BurlyWood);
            menuSpriteBatch.DrawString(menuFont, "QUIT", menuTextPosition, Color.White);

            menuSpriteBatch.End();
        }

    }
}
