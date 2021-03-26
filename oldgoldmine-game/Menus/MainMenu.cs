using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;


namespace oldgoldmine_game.Menus
{
    class MainMenu : Menu
    {
        private SpriteText gameTitle;

        private Button playButton;
        private Button optionsButton;
        private Button exitButton;

        private SpriteText highscoreText;

        private OptionsMenu optionsMenu = new OptionsMenu();
        private bool optionsActive;


        public override void Initialize(Viewport viewport, Texture2D background, Menu parent = null)
        {
            this.parent = null;
            this.menuBackground = background;
            this.semiTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.5f)), viewport.Bounds.Center.ToVector2(), viewport.Bounds.Size.ToVector2());

            Vector2 buttonSize = new Vector2(400, 110);

            optionsMenu.Initialize(viewport, background, this);


            // MAIN MENU LAYOUT SETUP

            gameTitle = new SpriteText(OldGoldMineGame.resources.gameTitleFont, "Old Gold Mine",
                Color.DarkGoldenrod, new Vector2(viewport.Width / 2, viewport.Height * 0.16f), SpriteText.TextAnchor.MiddleCenter);

            playButton = new Button(viewport.Bounds.Center.ToVector2() - new Vector2(0, 40), buttonSize, 
                OldGoldMineGame.resources.menuItemsFont, "PLAY", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            optionsButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 80), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "OPTIONS", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);

            exitButton = new Button(viewport.Bounds.Center.ToVector2() + new Vector2(0, 200), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "QUIT", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.menuButtonTextures, Color.BurlyWood);


            highscoreText = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Highscore: " + OldGoldMineGame.BestScore,
                new Color(120, 210, 50, 255), new Vector2(viewport.Width / 2, viewport.Height / 2 + buttonSize.Y * 3/2 + 150),
                SpriteText.TextAnchor.MiddleCenter);
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

                if (menuBackground != null)
                {
                    spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);
                    semiTransparencyLayer.Draw(spriteBatch);
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


        public override void Show()
        {
            highscoreText.Text = "Highscore: " + OldGoldMineGame.BestScore;

            playButton.Enabled = true;
            optionsButton.Enabled = true;
            exitButton.Enabled = true;
            optionsActive = false;
        }


        // Hide the OptionsMenu entity and switch back to the MainMenu Update/Draw
        public override void CloseSubmenu()
        {
            optionsActive = false;
        }
    }
}
