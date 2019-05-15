using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    class PauseMenu : Menu
    {

        private Button resumeButton;
        private Button menuButton;


        public override void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton)
        {
            this.menuBackground = background;


            // Pause menu layout setup
            Rectangle resumeButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 - elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            Rectangle menuButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 + elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);


            resumeButton = new Button(resumeButtonRectangle,
                new SpriteText(font, "RESUME", resumeButtonRectangle.Center.ToVector2(), SpriteText.TextAlignment.MiddleCenter),
                normalButton, highlightedButton);

            menuButton = new Button(menuButtonRectangle,
                new SpriteText(font, "BACK TO MENU", menuButtonRectangle.Center.ToVector2(), SpriteText.TextAlignment.MiddleCenter),
                normalButton, highlightedButton);
        }


        public override void Update(in OldGoldMineGame application)
        {
            resumeButton.Update();
            menuButton.Update();

            if (InputManager.PauseKeyPressed)
                application.ResumeGame();

            if (resumeButton.IsClicked())
                application.ResumeGame();
            else if (menuButton.IsClicked())
                application.ToMainMenu();
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.DarkGoldenrod);

            spriteBatch.Begin();

            if (menuBackground != null)
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);

            resumeButton.Draw(spriteBatch);
            menuButton.Draw(spriteBatch);

            spriteBatch.End();
        }

    }
}
