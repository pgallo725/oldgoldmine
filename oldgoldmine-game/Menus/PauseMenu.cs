using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.UI;

namespace OldGoldMine.Menus
{
    class PauseMenu : Menu
    {
        private SpriteText menuTitle;

        private Button resumeButton;
        private Button optionsButton;
        private Button menuButton;

        private readonly OptionsMenu optionsMenu = new OptionsMenu();
        private bool optionsActive;


        public override void Initialize(Viewport viewport, Texture2D background, Menu parent = null)
        {
            this.parent = parent;
            this.background = background;
            this.transparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.4f)), viewport.Bounds.Center, viewport.Bounds.Size);

            Point buttonSize = new Point(400, 120);

            optionsMenu.Initialize(viewport, background, this);


            // PAUSE MENU LAYOUT SETUP

            menuTitle = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "GAME PAUSED",
                Color.LightGray, new Point(viewport.Width / 2, (int)(viewport.Height * 0.12f)));

            resumeButton = new Button(viewport.Bounds.Center - new Point(0, 80), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "RESUME", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            optionsButton = new Button(viewport.Bounds.Center + new Point(0, 60), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "OPTIONS", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            menuButton = new Button(viewport.Bounds.Center + new Point(0, 200), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "BACK TO MENU", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.graphics.GraphicsDevice.Viewport;
            Point center = viewport.Bounds.Center;

            this.transparencyLayer.Area = viewport.Bounds;

            menuTitle.Position = new Point(viewport.Width / 2, (int)(viewport.Height * 0.12f));

            resumeButton.Position = center - new Point(0, 80);
            optionsButton.Position = center + new Point(0, 60);
            menuButton.Position = center + new Point(0, 200);
        }


        public override void Update()
        {
            if (!optionsActive)
            {
                if (resumeButton.Update() || InputManager.PausePressed)
                {
                    OldGoldMineGame.Application.ResumeGame();
                }
                else if (optionsButton.Update())
                {
                    optionsActive = true;
                    optionsMenu.Show();
                }
                else if (menuButton.Update())
                {
                    OldGoldMineGame.Application.ToMainMenu();
                }
            }
            else
            {
                // If the options menu is the one currently active, the Update call is forwarded
                // without running any code inside the PauseMenu
                optionsMenu.Update();
            }
        }


        public override void Show()
        {
            Layout();

            resumeButton.Enabled = true;
            menuButton.Enabled = true;
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            if (!optionsActive)
            {
                screen.Clear(Color.Black);

                spriteBatch.Begin();

                if (background != null)
                {
                    spriteBatch.Draw(background, screen.Viewport.Bounds, Color.White);
                    transparencyLayer.Draw(spriteBatch);
                }

                menuTitle.Draw(spriteBatch);

                resumeButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
                menuButton.Draw(spriteBatch);

                spriteBatch.End();
            }
            else
            {
                // If the options menu is the one currently active, it gets drawn instead of the PauseMenu
                optionsMenu.Draw(screen, spriteBatch);
            }
        }


        public override void CloseSubmenu()
        {
            Layout();   // If the display options have changed, the layout needs to be updated

            optionsActive = false;
        }
    }
}
