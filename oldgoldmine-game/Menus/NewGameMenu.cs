using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.Gameplay;
using OldGoldMine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace OldGoldMine.Menus
{
    class NewGameMenu : Menu
    {
        private readonly SpriteText titleText;

        private readonly SpriteText speedLabel;
        private readonly Selector speedToggle;

        private readonly SpriteText difficultyLabel;
        private readonly Selector difficultyToggle;

        private readonly SpriteText seedLabel;
        private readonly TextBox seedBox;

        private readonly Image cartPanel;
        private readonly Image cartPreview;
        private readonly Selector cartSelector;
        private readonly SpriteText cartLockedLabel;
        private readonly Image cartTransparencyLayer;
        private readonly Image cartLockedIcon;

        private readonly SpriteText scoreMultiplierLabel;

        private readonly Button startButton;
        private readonly Button backButton;

        
        private readonly int[] cartPointsNeeded = { -1, 2500, 7500, 15000, 25000 };


        // Level parameters
        private int SelectedSpeed { get { return (20 + speedToggle.SelectedValueIndex * 10); } }
        private int SelectedDifficulty { get { return difficultyToggle.SelectedValueIndex; } }
        private int SelectedCart { get { return cartSelector.SelectedValueIndex; } }
        private long Seed
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


        public NewGameMenu(Viewport viewport, Texture2D background, Menu parent = null)
            : base(background, new SolidColorTexture(OldGoldMineGame.Graphics.GraphicsDevice,
                new Color(Color.Black, 0.66f)), new Point(240, 80), parent)
        {
            Point anchorPointTitle = new Point((int)(viewport.Width * 0.5f), (int)(viewport.Height * 0.08f));
            Point anchorPointCarts = new Point((int)(viewport.Width * 0.25f), (int)(viewport.Height * 0.45f));
            Point anchorPointSettings = new Point((int)(viewport.Width * 0.75f), (int)(viewport.Height * 0.45f));
            Point anchorPointScore = new Point((int)(viewport.Width * 0.25f), (int)(viewport.Height * 0.875f));      
            Point anchorPointButtons = new Point((int)(viewport.Width * 0.75f), (int)(viewport.Height * 0.875f));

            // CUSTOMIZATION MENU LAYOUT SETUP

            backButton = new Button(anchorPointButtons - new Point(buttonSize.X / 2 + 10, 0), buttonSize,
                Resources.GetFont("MenuItem"), "BACK", Color.LightGoldenrodYellow,
                Resources.GetSpritePack("StandardButton"), Color.BurlyWood);

            startButton = new Button(anchorPointButtons + new Point(buttonSize.X / 2 + 10, 0), buttonSize,
                Resources.GetFont("MenuItem"), "START", Color.LightGoldenrodYellow,
                Resources.GetSpritePack("StandardButton"), Color.BurlyWood);


            titleText = new SpriteText(Resources.GetFont("MenuTitle"), "CUSTOMIZE YOUR GAME",
                Color.DarkOrange, anchorPointTitle, SpriteText.TextAnchor.MiddleCenter);


            difficultyLabel = new SpriteText(Resources.GetFont("MenuItem"), "Difficulty",
                Color.White, anchorPointSettings - new Point(250, 0), SpriteText.TextAnchor.MiddleRight);

            difficultyToggle = new Selector(difficultyLabel.Position + new Point(300, 0), new Point(70, 70), 240,
                Resources.GetFont("MenuItem"), new List<string>() { "Easy", "Medium", "Hard" }, Color.White,
                Resources.GetSpritePack("LeftArrowButton"), Resources.GetSpritePack("RightArrowButton"), Color.BurlyWood);

            speedLabel = new SpriteText(Resources.GetFont("MenuItem"), "Starting\n   speed",
                Color.White, difficultyLabel.Position - new Point(0, 150), SpriteText.TextAnchor.MiddleRight);

            speedToggle = new Selector(speedLabel.Position + new Point(300, 0), new Point(60, 60), 180,
                Resources.GetFont("MenuItem"), new List<string>() { "20", "30", "40", "50", "60", "70", "80" }, Color.White,
                Resources.GetSpritePack("MinusButton"), Resources.GetSpritePack("PlusButton"), Color.BurlyWood);

            seedLabel = new SpriteText(Resources.GetFont("MenuItem"), "Seed",
                Color.White, difficultyLabel.Position + new Point(0, 150), SpriteText.TextAnchor.MiddleRight);

            seedBox = new TextBox(seedLabel.Position + new Point(300, 0), new Point(320, 100), new Point(32, 20),
                Resources.GetSpritePack("Textbox"), Resources.GetFont("MenuItem"), Color.White, charLimit: 10, shade: Color.BurlyWood);


            cartPanel = new Image(Resources.GetTexture("FramedPanel"), anchorPointCarts, new Point(450, 440));

            cartPreview = new Image(Resources.GetTexture("CartPreview_0"), cartPanel.Position, new Point(380, 380));

            cartTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.Graphics.GraphicsDevice,
                new Color(Color.Black, 0.6f)), cartPanel.Position, new Point(400, 400));

            cartSelector = new Selector(cartPanel.Position + new Point(0, 270), new Point(60, 60), 200,
                Resources.GetFont("HUD"), new List<string>() { "Woody", "Ol' Rusty", "Thunberg", "The Tank", "G.R.O.D.T." }, Color.White,
                Resources.GetSpritePack("LeftArrowButton"), Resources.GetSpritePack("RightArrowButton"), Color.BurlyWood);

            cartLockedLabel = new SpriteText(Resources.GetFont("MenuSmall"), "Unlock with score ",
                Color.DarkGray, cartSelector.Position + new Point(0, 65));

            cartLockedIcon = new Image(Resources.GetTexture("LockIcon"), cartPanel.Position, new Point(300, 300));


            scoreMultiplierLabel = new SpriteText(Resources.GetFont("MenuItem"), "Score multiplier: 1.0x",
                Color.White, anchorPointScore, SpriteText.TextAnchor.MiddleLeft);
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.Graphics.GraphicsDevice.Viewport;

            Point anchorPointTitle = new Point((int)(viewport.Width * 0.5f), (int)(viewport.Height * 0.08f));
            Point anchorPointCarts = new Point((int)(viewport.Width * 0.25f), (int)(viewport.Height * 0.45f));
            Point anchorPointSettings = new Point((int)(viewport.Width * 0.75f), (int)(viewport.Height * 0.45f));
            Point anchorPointScore = new Point((int)(viewport.Width * 0.25f), (int)(viewport.Height * 0.875f));
            Point anchorPointButtons = new Point((int)(viewport.Width * 0.75f), (int)(viewport.Height * 0.875f));

            backButton.Position = anchorPointButtons - new Point(buttonSize.X / 2 + 10, 0);
            startButton.Position = anchorPointButtons + new Point(buttonSize.X / 2 + 10, 0);

            titleText.Position = anchorPointTitle;

            difficultyLabel.Position = anchorPointSettings - new Point(250, 0);
            difficultyToggle.Position = difficultyLabel.Position + new Point(300, 0);

            speedLabel.Position = difficultyLabel.Position - new Point(0, 150);
            speedToggle.Position = speedLabel.Position + new Point(300, 0);

            seedLabel.Position = difficultyLabel.Position + new Point(0, 150);
            seedBox.Position = seedLabel.Position + new Point(300, 0);

            cartPanel.Position = anchorPointCarts;
            cartPreview.Position = cartPanel.Position;
            cartTransparencyLayer.Position = cartPanel.Position;
            cartSelector.Position = cartPanel.Position + new Point(0, 270);
            cartLockedLabel.Position = cartSelector.Position + new Point(0, 65);
            cartLockedIcon.Position = cartPanel.Position;

            scoreMultiplierLabel.Position = anchorPointScore;
        }


        public override void Show()
        {
            Layout();

            // Reset all the UI elements to their original status

            speedToggle.SelectedValueIndex = 2;
            difficultyToggle.SelectedValueIndex = 1;

            seedBox.Content = string.Empty;

            cartSelector.SelectedValueIndex = 0;

            UpdateSettingsDisplay();
        }


        public override void Update()
        {
            speedToggle.Update();
            difficultyToggle.Update();
            seedBox.Update();
            cartSelector.Update();

            UpdateSettingsDisplay();

            if (startButton.Update() || InputManager.EnterPressed)
            {
                OldGoldMineGame.Application.StartGame(new GameSettings
                    (SelectedSpeed, SelectedDifficulty, Seed, SelectedCart));
            }
            else if (backButton.Update() || InputManager.BackPressed)
            {
                parent.Show();
            }
        }


        private float ComputeMultiplier()
        {
            int baseSpeed = 20 + SelectedDifficulty * 2 * 10;
            return 1f + (SelectedDifficulty - 1) * 0.5f + (SelectedSpeed - baseSpeed) / 10 * 0.05f;
        }

        private void UpdateSettingsDisplay()
        {
            // Update cart model preview 
            cartPreview.ImageTexture = Resources.GetTexture($"CartPreview_{SelectedCart}");

            if (Score.Best > cartPointsNeeded[SelectedCart])
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

            // Update score multiplier
            float scoreMultiplier = ComputeMultiplier();

            if (scoreMultiplier > 1f)
                scoreMultiplierLabel.Color = new Color(170, 240, 60, 255);    // Green tint
            else if (scoreMultiplier < 1f)
                scoreMultiplierLabel.Color = new Color(170, 0, 0, 255);       // Red tint
            else scoreMultiplierLabel.Color = Color.White;

            scoreMultiplierLabel.Text = scoreMultiplier.ToString("Score multiplier: 0.00#x");
        }


        public override void Draw(in SpriteBatch spriteBatch)
        {
            var screen = spriteBatch.GraphicsDevice;
            screen.Clear(Color.Black);

            spriteBatch.Begin();

            if (background != null)
            {
                spriteBatch.Draw(background, screen.Viewport.Bounds, Color.White);
                spriteBatch.Draw(middleLayer, screen.Viewport.Bounds, Color.White);
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

    }
}
