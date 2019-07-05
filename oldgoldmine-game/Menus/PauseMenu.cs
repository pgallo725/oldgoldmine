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


        public override void Initialize(Viewport viewport, Texture2D background)
        {
            this.menuBackground = background;

            Vector2 buttonSize = new Vector2(400, 120);


            // PAUSE MENU LAYOUT SETUP

            resumeButton = new Button(viewport.Bounds.Center.ToVector2() - new Vector2(0, 75), buttonSize,
                OldGoldMineGame.resources.menuButtonFont, "RESUME", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);

            menuButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 75), buttonSize,
                OldGoldMineGame.resources.menuButtonFont, "BACK TO MENU", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);
        }


        public override void Update()
        {
            resumeButton.Update();
            menuButton.Update();

            if (InputManager.PauseKeyPressed)
                OldGoldMineGame.Application.ResumeGame();

            if (resumeButton.IsClicked())
                OldGoldMineGame.Application.ResumeGame();
            else if (menuButton.IsClicked())
                OldGoldMineGame.Application.ToMainMenu();
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


        public override void Show()
        {
            resumeButton.Enabled = true;
            menuButton.Enabled = true;
        }
    }
}
