using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.Gameplay;
using OldGoldMine.UI;


namespace OldGoldMine.Menus
{
    class MainMenu : Menu
    {
        private readonly SpriteText gameTitle;

        private readonly Button playButton;
        private readonly Button optionsButton;
        private readonly Button exitButton;

        private readonly SpriteText highscoreText;

        // SUB-MENUS
        private readonly NewGameMenu newGameMenu;
        private readonly OptionsMenu optionsMenu;
        private bool newGameMenuActive;
        private bool optionsMenuActive;


        public MainMenu(Viewport viewport, Texture2D background, Menu parent = null)
            : base(background, new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.5f)), new Point(400, 110), parent)
        {
            // CREATE SUB-MENUS

            newGameMenu = new NewGameMenu(viewport, background, this);
            optionsMenu = new OptionsMenu(viewport, background, this);

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

            highscoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Highscore: " + Score.Best,
                new Color(120, 210, 50, 255), new Point(viewport.Width / 2, viewport.Height / 2 + (int)(buttonSize.Y * 1.5f) + 150));
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.graphics.GraphicsDevice.Viewport;

            gameTitle.Position = new Point(viewport.Width / 2, (int)(viewport.Height * 0.16f));

            playButton.Position = viewport.Bounds.Center - new Point(0, 40);
            optionsButton.Position = viewport.Bounds.Center + new Point(0, 80);
            exitButton.Position = viewport.Bounds.Center + new Point(0, 200);

            highscoreText.Position = new Point(viewport.Width / 2, viewport.Height / 2 + (int)(buttonSize.Y * 1.5f) + 150);
        }


        public override void Show()
        {
            Layout();

            highscoreText.Text = "Highscore: " + Score.Best;

            playButton.Enabled = true;
            optionsButton.Enabled = true;
            exitButton.Enabled = true;
            optionsMenuActive = false;
            newGameMenuActive = false;
        }


        public override void Update()
        {
            if (optionsMenuActive)
            {
                // If the options menu is the one currently active, the Update call is forwarded
                // without running any code inside the MainMenu
                optionsMenu.Update();
            }
            else if (newGameMenuActive)
            {
                // If the new game menu is the one currently active, the Update call is forwarded
                // without running any code inside the MainMenu
                newGameMenu.Update();
            }
            else
            {
                if (playButton.Update())
                {
                    newGameMenuActive = true;
                    newGameMenu.Show();
                }
                else if (optionsButton.Update())
                {
                    optionsMenuActive = true;
                    optionsMenu.Show();
                }
                else if (exitButton.Update())
                {
                    OldGoldMineGame.Application.Exit();
                }
            }
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            if (optionsMenuActive)
            {
                // If the options menu is the one currently active, it gets drawn instead of the MainMenu
                optionsMenu.Draw(screen, spriteBatch);
            }
            else if (newGameMenuActive)
            {
                // If the new game menu is the one currently active, it gets drawn instead of the MainMenu
                newGameMenu.Draw(screen, spriteBatch);
            }
            else
            {
                screen.Clear(Color.Black);

                spriteBatch.Begin();

                if (background != null)
                {
                    spriteBatch.Draw(background, screen.Viewport.Bounds, Color.White);
                    spriteBatch.Draw(middleLayer, screen.Viewport.Bounds, Color.White);
                }

                gameTitle.Draw(spriteBatch);

                playButton.Draw(spriteBatch);
                optionsButton.Draw(spriteBatch);
                exitButton.Draw(spriteBatch);

                highscoreText.Draw(spriteBatch);

                spriteBatch.End();
            }
        }

    }
}
