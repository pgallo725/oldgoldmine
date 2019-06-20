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

        public override void Initialize(GraphicsDevice device, Texture2D background)
        {
            this.menuBackground = background;

            // Game over menu layout setup
            Rectangle resumeButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 - elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);

            Rectangle menuButtonRectangle = new Rectangle(device.Viewport.Width / 2 - (int)buttonSize.X / 2,
                device.Viewport.Height / 2 - (int)buttonSize.Y / 2 + elementSeparation,
                (int)buttonSize.X, (int)buttonSize.Y);


            replayButton = new Button(resumeButtonRectangle, 
                OldGoldMineGame.resources.menuButtonFont, "PLAY AGAIN", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);

            menuButton = new Button(menuButtonRectangle, 
                OldGoldMineGame.resources.menuButtonFont, "BACK TO MENU", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);
        }


        public override void Update()
        {
            replayButton.Update();
            menuButton.Update();

            if (InputManager.PauseKeyPressed)
                OldGoldMineGame.Application.ResumeGame();

            if (replayButton.IsClicked())
                OldGoldMineGame.Application.StartGame();
            else if (menuButton.IsClicked())
                OldGoldMineGame.Application.ToMainMenu();
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

        public override void Show()
        {
            return;
        }
    }
}
