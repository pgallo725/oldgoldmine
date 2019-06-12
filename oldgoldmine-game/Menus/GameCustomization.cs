using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.UI;


namespace oldgoldmine_game.Menus
{
    class GameCustomization : Menu
    {
        private Button startButton;
        private Button backButton;

        private SpriteText titleText;


        public override void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton)
        {
            this.menuBackground = background;
            this.buttonSize = new Vector2(240, 80);

            // Customization menu layout setup
            Rectangle startButtonRectangle = new Rectangle(device.Viewport.Width - (elementSeparation + 100) - (int)(buttonSize.X / 2),
                device.Viewport.Height - (elementSeparation + 10) - (int)(buttonSize.Y / 2),
                (int)buttonSize.X, (int)buttonSize.Y);

            Rectangle backButtonRectangle = new Rectangle(startButtonRectangle.X - (elementSeparation + 75) - (int)buttonSize.X / 2,
                device.Viewport.Height - (elementSeparation + 10) - (int)(buttonSize.Y / 2),
                (int)buttonSize.X, (int)buttonSize.Y);

            Vector2 titlePosition = new Vector2(device.Viewport.Width / 2, elementSeparation);


            startButton = new Button(startButtonRectangle, font, "START", normalButton, highlightedButton);
            backButton = new Button(backButtonRectangle, font, "BACK", normalButton, highlightedButton);

            titleText = new SpriteText(font, "CUSTOMIZE YOUR GAME", Color.DarkOrange,
                titlePosition, SpriteText.TextAlignment.MiddleCenter);
        }


        public override void Update()
        {
            startButton.Update();
            backButton.Update();

            if (startButton.IsClicked())
                OldGoldMineGame.Application.StartGame();
            else if (backButton.IsClicked())
                OldGoldMineGame.Application.ToMainMenu();
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.Black);

            spriteBatch.Begin();

            if (menuBackground != null)
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);

            startButton.Draw(spriteBatch);
            backButton.Draw(spriteBatch);

            titleText.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
