using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    class PauseMenu : Menu
    {
        private SpriteText menuTitle;

        private Button resumeButton;
        private Button menuButton;


        public override void Initialize(Viewport viewport, Texture2D background)
        {
            this.menuBackground = background;
            this.semiTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.4f)), viewport.Bounds.Center.ToVector2(), viewport.Bounds.Size.ToVector2());

            Vector2 buttonSize = new Vector2(400, 120);


            // PAUSE MENU LAYOUT SETUP

            menuTitle = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "GAME PAUSED",
                Color.LightGray, new Vector2(viewport.Width / 2, viewport.Height * 0.1f), SpriteText.TextAnchor.MiddleCenter);

            resumeButton = new Button(viewport.Bounds.Center.ToVector2() - new Vector2(0, 75), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "RESUME", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures);

            menuButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 75), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "BACK TO MENU", Color.LightGoldenrodYellow,
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
            screen.Clear(Color.Black);

            spriteBatch.Begin();

            if (menuBackground != null)
            {
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);
                semiTransparencyLayer.Draw(spriteBatch);
            }

            menuTitle.Draw(spriteBatch);

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
