using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;


namespace oldgoldmine_game.Menus
{
    class CustomizationMenu : Menu
    {
        private SpriteText titleText;

        private SpriteText speedLabel;
        private ToggleSelector speedToggle;

        private SpriteText difficultyLabel;
        private ToggleSelector difficultyToggle;

        private SpriteText seedLabel;
        private TextBox seedBox;

        private Image cartPanel;
        private Image cartPreview;
        private ToggleSelector cartSelector;
        private SpriteText cartLockedLabel;
        private Image cartTransparencyLayer;
        private Image cartLockedIcon;

        private SpriteText scoreMultiplierLabel;

        private Button startButton;
        private Button backButton;

        
        private int[] cartPointsNeeded = { -1, 2500, 8000, 20000 };


        private Vector2 anchorPointTitle;
        private Vector2 anchorPointCarts;
        private Vector2 anchorPointSettings;
        private Vector2 anchorPointScore;
        private Vector2 anchorPointButtons;


        // Level parameters
        private float scoreMultiplier = 1f;

        public int SelectedSpeed { get { return (20 + speedToggle.SelectedValueIndex * 10); } }
        public int SelectedDifficulty { get { return difficultyToggle.SelectedValueIndex; } }
        public float ScoreMultiplier { get { return scoreMultiplier; } }
        public int SelectedCart { get { return cartSelector.SelectedValueIndex; } }
        public long Seed
        {
            get
            {
                if (seedBox.Content.Length == 0)
                    return System.DateTime.Now.Ticks;

                MD5 md5Hasher = MD5.Create();
                var hashed = md5Hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(seedBox.Content));
                return System.BitConverter.ToInt64(hashed, 0);
            }
        }


        public override void Initialize(Viewport viewport, Texture2D background)
        {
            this.menuBackground = background;
            this.semiTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.66f)), viewport.Bounds.Center.ToVector2(), viewport.Bounds.Size.ToVector2());


            Vector2 buttonSize = new Vector2(240, 80);

            float screenWidth = viewport.Width;
            float screenHeight = viewport.Height;

            anchorPointTitle = new Vector2(screenWidth * 0.5f, screenHeight * 0.075f);          // Position at center X and 7.5% Y from the top
            anchorPointCarts = new Vector2(screenWidth * 0.25f, screenHeight * 0.45f);          // Position at -25% X and -5% Y from the center
            anchorPointSettings = new Vector2(screenWidth * 0.75f, screenHeight * 0.45f);       // Position at +25% X and -5% Y from the center
            anchorPointScore = new Vector2(screenWidth * 0.25f, screenHeight * 0.875f);         // Position at -25% X and -37.5% Y from the center
            anchorPointButtons = new Vector2(screenWidth * 0.75f, screenHeight * 0.875f);       // Position at +25% X and -37.5% Y from the center


            // CUSTOMIZATION MENU LAYOUT SETUP

            backButton = new Button(anchorPointButtons - new Vector2(buttonSize.X / 2f + 10f, 0), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "BACK", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.standardButtonTextures);

            startButton = new Button(anchorPointButtons + new Vector2(buttonSize.X / 2f + 10f, 0), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "START", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.standardButtonTextures);


            titleText = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "CUSTOMIZE YOUR GAME", Color.DarkOrange,
                anchorPointTitle, SpriteText.TextAnchor.MiddleCenter);


            difficultyLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Difficulty",
                anchorPointSettings - new Vector2(250, 0), SpriteText.TextAnchor.MiddleRight);

            difficultyToggle = new ToggleSelector(difficultyLabel.Position + new Vector2(300, 0), new Vector2(70, 70), 240,
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures,
                OldGoldMineGame.resources.menuItemsFont, new List<string>() { "Easy", "Medium", "Hard" });

            speedLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Starting\n     speed",
                difficultyLabel.Position - new Vector2(0, 150), SpriteText.TextAnchor.MiddleRight);

            speedToggle = new ToggleSelector(speedLabel.Position + new Vector2(300, 0), new Vector2(60, 60), 180,
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures,
                OldGoldMineGame.resources.menuItemsFont, new List<string>() { "20", "30", "40", "50", "60", "70", "80" });

            seedLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Seed",
                difficultyLabel.Position + new Vector2(0, 150), SpriteText.TextAnchor.MiddleRight);

            seedBox = new TextBox(seedLabel.Position + new Vector2(300, 0), new Vector2(320, 100), new Vector2(32, 20),
                OldGoldMineGame.resources.textboxTextures, OldGoldMineGame.resources.menuItemsFont, characterLimit: 10);


            cartPanel = new Image(OldGoldMineGame.resources.framedPanelTexture, anchorPointCarts, new Vector2(450, 440));

            cartPreview = new Image(OldGoldMineGame.resources.cartPreviewImages[0], cartPanel.Position, new Vector2(380, 380));

            cartTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.6f)), cartPanel.Position, new Vector2(400, 400));

            cartSelector = new ToggleSelector(cartPanel.Position + new Vector2(0, 270), new Vector2(60, 60), 200,
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures,
                OldGoldMineGame.resources.hudFont, new List<string>() { "Cart.001", "Cart.002", "Cart.003", "Cart.004" });

            cartLockedLabel = new SpriteText(OldGoldMineGame.resources.menuSmallFont, "Unlock with score ", Color.DarkGray,
                cartSelector.Position + new Vector2(0, 60));

            cartLockedIcon = new Image(OldGoldMineGame.resources.lockIcon, cartPanel.Position, new Vector2(300, 300));


            scoreMultiplierLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Score multiplier: 1.0x",
                anchorPointScore, SpriteText.TextAnchor.MiddleLeft);
        }


        public override void Update()
        {
            speedToggle.Update();
            difficultyToggle.Update();
            seedBox.Update();
            cartSelector.Update();
            startButton.Update();
            backButton.Update();

            UpdateScoreMultiplier();
            UpdateCartSelection();

            if (startButton.IsClicked() || InputManager.EnterKeyPressed )
            {
                OldGoldMineGame.GameSettings newGameSettings = new OldGoldMineGame.GameSettings(
                    ScoreMultiplier, SelectedSpeed, SelectedDifficulty, Seed, SelectedCart);

                OldGoldMineGame.Application.StartGame(newGameSettings);
            }
            else if (backButton.IsClicked() || InputManager.BackKeyPressed )
            {
                OldGoldMineGame.Application.ToMainMenu();
            }
        }


        private void UpdateScoreMultiplier()
        {
            scoreMultiplier = 1f + (difficultyToggle.SelectedValueIndex - 1) * 0.5f
                + speedToggle.SelectedValueIndex * 0.05f;

            scoreMultiplierLabel.Text = scoreMultiplier.ToString("Score multiplier: 0.00#x");
        }

        private void UpdateCartSelection()
        {
            cartPreview.ImageTexture = OldGoldMineGame.resources.cartPreviewImages[SelectedCart];

            if (OldGoldMineGame.BestScore > cartPointsNeeded[SelectedCart])
            {
                cartTransparencyLayer.Enabled = false;
                cartLockedLabel.Enabled = false;
                cartLockedIcon.Enabled = false;
                startButton.Enabled = true;
            }
            else
            {
                cartTransparencyLayer.Enabled = true;
                cartLockedLabel.Text = cartPointsNeeded[SelectedCart].ToString("Unlock with score > 0.#");
                cartLockedLabel.Enabled = true;
                cartLockedIcon.Enabled = true;
                startButton.Enabled = false;
            }
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

            titleText.Draw(spriteBatch);

            speedLabel.Draw(spriteBatch);
            speedToggle.Draw(spriteBatch);

            difficultyLabel.Draw(spriteBatch);
            difficultyToggle.Draw(spriteBatch);

            seedLabel.Draw(spriteBatch);
            seedBox.Draw(spriteBatch);

            cartPanel.Draw(spriteBatch);
            cartPreview.Draw(spriteBatch);
            cartSelector.Draw(spriteBatch);
            cartLockedLabel.Draw(spriteBatch);
            cartTransparencyLayer.Draw(spriteBatch);
            cartLockedIcon.Draw(spriteBatch);

            scoreMultiplierLabel.Draw(spriteBatch);

            startButton.Draw(spriteBatch);
            backButton.Draw(spriteBatch);

            spriteBatch.End();
        }


        // Reset all the UI elements and selectable values to their original status
        public override void Show()
        {
            speedToggle.SelectedValueIndex = 0;
            difficultyToggle.SelectedValueIndex = 1;

            seedBox.Content = string.Empty;

            cartSelector.SelectedValueIndex = 0;

            UpdateScoreMultiplier();
            UpdateCartSelection();
        }
    }
}
