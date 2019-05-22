using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    class GameOverMenu : Menu
    {

        private Button replayButton;
        private Button menuButton;

        public override void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton)
        {
            this.menuBackground = background;


            // Game over menu layout setup
            Rectangle resumeButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 - elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            Rectangle menuButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 + elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);


            replayButton = new Button(resumeButtonRectangle, font, "PLAY AGAIN", normalButton, highlightedButton);
            menuButton = new Button(menuButtonRectangle,font, "BACK TO MENU", normalButton, highlightedButton);
        }


        public override void Update(in OldGoldMineGame application)
        {
            replayButton.Update();
            menuButton.Update();

            if (InputManager.PauseKeyPressed)
                application.ResumeGame();

            if (replayButton.IsClicked())
                application.StartGame();
            else if (menuButton.IsClicked())
                application.ToMainMenu();
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.DarkRed);

            spriteBatch.Begin();

            if (menuBackground != null)
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);

            replayButton.Draw(spriteBatch);
            menuButton.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
