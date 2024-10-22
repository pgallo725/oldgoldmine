﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.UI;

namespace OldGoldMine.Menus
{
    class PauseMenu : Menu
    {
        private readonly SpriteText menuTitle;

        private readonly Button resumeButton;
        private readonly Button optionsButton;
        private readonly Button menuButton;

        // SUB-MENU
        private readonly OptionsMenu optionsMenu;
        private bool optionsActive;


        public PauseMenu(Viewport viewport, Texture2D background, Menu parent = null)
            : base(background, new SolidColorTexture(OldGoldMineGame.Graphics.GraphicsDevice,
                new Color(Color.Black, 0.4f)), new Point(400, 120), parent)
        {
            // CREATE SUB-MENU

            optionsMenu = new OptionsMenu(viewport, background, this);

            // PAUSE MENU LAYOUT SETUP

            menuTitle = new SpriteText(Resources.GetFont("MenuTitle"), "GAME PAUSED",
                Color.LightGray, new Point(viewport.Width / 2, (int)(viewport.Height * 0.12f)));

            resumeButton = new Button(viewport.Bounds.Center - new Point(0, 80), buttonSize,
                Resources.GetFont("MenuItem"), "RESUME", Color.LightGoldenrodYellow,
                Resources.GetSpritePack("MainButton"), Color.BurlyWood);

            optionsButton = new Button(viewport.Bounds.Center + new Point(0, 60), buttonSize,
                Resources.GetFont("MenuItem"), "OPTIONS", Color.LightGoldenrodYellow,
                Resources.GetSpritePack("MainButton"), Color.BurlyWood);

            menuButton = new Button(viewport.Bounds.Center + new Point(0, 200), buttonSize,
                Resources.GetFont("MenuItem"), "BACK TO MENU", Color.LightGoldenrodYellow,
                Resources.GetSpritePack("MainButton"), Color.BurlyWood);
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.Graphics.GraphicsDevice.Viewport;

            menuTitle.Position = new Point(viewport.Width / 2, (int)(viewport.Height * 0.12f));

            resumeButton.Position = viewport.Bounds.Center - new Point(0, 80);
            optionsButton.Position = viewport.Bounds.Center + new Point(0, 60);
            menuButton.Position = viewport.Bounds.Center + new Point(0, 200);
        }


        public override void Update()
        {
            if (optionsActive)
            {
                // If the options menu is the one currently active, the Update call is forwarded
                // without running any code inside the PauseMenu
                optionsMenu.Update();
            }
            else
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
        }


        public override void Show()
        {
            Layout();

            resumeButton.Enabled = true;
            menuButton.Enabled = true;
            optionsActive = false;
        }


        public override void Draw(in SpriteBatch spriteBatch)
        {
            if (optionsActive)
            {
                // If the options menu is the one currently active, it gets drawn instead of the PauseMenu
                optionsMenu.Draw(spriteBatch);
            }
            else
            {
                var screen = spriteBatch.GraphicsDevice;
                screen.Clear(Color.Black);

                spriteBatch.Begin();

                if (background != null)
                {
                    spriteBatch.Draw(background, screen.Viewport.Bounds, Color.White);
                    spriteBatch.Draw(middleLayer, screen.Viewport.Bounds, Color.White);
                }

                menuTitle.Draw(spriteBatch);

                resumeButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
                menuButton.Draw(spriteBatch);

                spriteBatch.End();
            }
        }
    }
}
