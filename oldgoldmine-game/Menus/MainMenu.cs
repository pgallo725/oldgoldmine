using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.UI;


namespace OldGoldMine.Menus
{
    class MainMenu : Menu
    {
        private SpriteText gameTitle;

        private Button playButton;
        private Button optionsButton;
        private Button exitButton;

        private SpriteText highscoreText;

        // Sub-menu
        private readonly OptionsMenu optionsMenu = new OptionsMenu();
        private bool optionsActive;


        public override void Initialize(Viewport viewport, Texture2D background, Menu parent = null)
        {
            this.parent = null;
            this.background = background;
            this.transparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.5f)), viewport.Bounds.Center, viewport.Bounds.Size);

            Point buttonSize = new Point(400, 110);

            optionsMenu.Initialize(viewport, background, this);


            // MAIN MENU LAYOUT SETUP

            gameTitle = new SpriteText(OldGoldMineGame.resources.gameTitleFont, "Old Gold Mine",
                Color.DarkGoldenrod, new Point(viewport.Width / 2, (int)(viewport.Height * 0.16f)));

            playButton = new Button(viewport.Bounds.Center - new Point(0, 40), buttonSize, 
                OldGoldMineGame.resources.menuItemsFont, "PLAY", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            optionsButton = new Button(viewport.Bounds.Center + new Point(0, 80), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "OPTIONS", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            exitButton = new Button(viewport.Bounds.Center + new Point(0, 200), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "QUIT", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            highscoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Highscore: " + OldGoldMineGame.BestScore,
                new Color(120, 210, 50, 255), new Point(viewport.Width / 2, viewport.Height / 2 + buttonSize.Y * 3/2 + 150));
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.graphics.GraphicsDevice.Viewport;
            Point center = viewport.Bounds.Center;
            Point buttonSize = new Point(400, 110);

            this.transparencyLayer.Area = viewport.Bounds;

            gameTitle.Position = new Point(viewport.Width / 2, (int)(viewport.Height * 0.16f));

            playButton.Position = center - new Point(0, 40);
            optionsButton.Position = center + new Point(0, 80);
            exitButton.Position = center + new Point(0, 200);

            highscoreText.Position = new Point(viewport.Width / 2, viewport.Height / 2 + buttonSize.Y * 3/2 + 150);
        }


        public override void Show()
        {
            Layout();

            highscoreText.Text = "Highscore: " + OldGoldMineGame.BestScore;

            playButton.Enabled = true;
            optionsButton.Enabled = true;
            exitButton.Enabled = true;
            optionsActive = false;
        }


        public override void Update()
        {
            if (!optionsActive)
            {
                if (playButton.Update())
                {
                    OldGoldMineGame.Application.NewGame();
                }
                else if (optionsButton.Update())
                {
                    optionsActive = true;
                    optionsMenu.Show();
                }
                else if (exitButton.Update())
                {
                    OldGoldMineGame.Application.Exit();
                }
            }
            else
            {
                // If the options menu is the one currently active, the Update call is forwarded
                // without running any code inside the MainMenu
                optionsMenu.Update();
            }
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

                gameTitle.Draw(spriteBatch);

                playButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
                exitButton.Draw(spriteBatch);

                highscoreText.Draw(spriteBatch);

                spriteBatch.End();
            }
            else
            {
                // If the options menu is the one currently active, it gets drawn instead of the MainMenu
                optionsMenu.Draw(screen, spriteBatch);
            }
        }


        // Hide the OptionsMenu entity and switch back to the MainMenu Update/Draw
        public override void CloseSubmenu()
        {
            Layout();   // If the display options have changed, the layout needs to be updated

            optionsActive = false;
        }
    }
}
