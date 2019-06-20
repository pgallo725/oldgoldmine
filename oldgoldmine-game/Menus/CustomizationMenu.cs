using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private ToggleSelector cartSelector;

        private SpriteText scoreMultiplierLabel;

        private Button startButton;
        private Button backButton;


        // Level parameters
        private float scoreMultiplier = 1f;

        public int SelectedSpeed { get { return (20 + speedToggle.SelectedValueIndex * 10); } }
        public int SelectedDifficulty { get { return difficultyToggle.SelectedValueIndex; } }
        public float ScoreMultiplier { get { return scoreMultiplier; } }


        public override void Initialize(GraphicsDevice device, Texture2D background)
        {
            this.menuBackground = background;
            this.buttonSize = new Vector2(240, 80);

            // CUSTOMIZATION MENU LAYOUT SETUP

            backButton = new Button(new Vector2(device.Viewport.Width * 0.75f - (buttonSize.X + 20) / 2f,
                device.Viewport.Height * 0.875f), buttonSize,
                OldGoldMineGame.resources.menuButtonFont, "BACK", Color.White,
                OldGoldMineGame.resources.standardButtonTextures);

            startButton = new Button(new Vector2(device.Viewport.Width * 0.75f + (buttonSize.X + 20) / 2f,
                device.Viewport.Height * 0.875f), buttonSize,
                OldGoldMineGame.resources.menuButtonFont, "START", Color.White,
                OldGoldMineGame.resources.standardButtonTextures);


            titleText = new SpriteText(OldGoldMineGame.resources.menuButtonFont, "CUSTOMIZE YOUR GAME", Color.DarkOrange,
                new Vector2(device.Viewport.Width / 2, elementSeparation), SpriteText.TextAnchor.MiddleCenter);


            speedLabel = new SpriteText(OldGoldMineGame.resources.hudFont, "Starting\n     speed",
                new Vector2(device.Viewport.Width / 2f + 130, device.Viewport.Height / 4f + 20), SpriteText.TextAnchor.MiddleRight);

            speedToggle = new ToggleSelector(speedLabel.Position + new Vector2(300, 0), new Vector2(60, 60), 180,
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures,
                OldGoldMineGame.resources.hudFont, new List<string>() { "20", "30", "40", "50", "60" });


            difficultyLabel = new SpriteText(OldGoldMineGame.resources.hudFont, "Difficulty", 
                speedLabel.Position + new Vector2(0, 150), SpriteText.TextAnchor.MiddleRight);

            difficultyToggle = new ToggleSelector(difficultyLabel.Position + new Vector2(300, 0), new Vector2(70, 70), 240,
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures, 
                OldGoldMineGame.resources.hudFont, new List<string>() { "Easy", "Medium", "Hard" });
            

            seedLabel = new SpriteText(OldGoldMineGame.resources.hudFont, "Seed",
                difficultyLabel.Position + new Vector2(0, 150), SpriteText.TextAnchor.MiddleRight);

            seedBox = new TextBox(seedLabel.Position + new Vector2(300, 0), new Vector2(320, 100), new Vector2(32, 20),
                OldGoldMineGame.resources.textboxTextures, OldGoldMineGame.resources.hudFont, characterLimit: 10);


            cartPanel = new Image(OldGoldMineGame.resources.plusButtonTextures.normal,
                new Vector2(device.Viewport.Width / 4, device.Viewport.Height / 2.5f + 20), new Vector2(500, 500));

            cartSelector = new ToggleSelector(cartPanel.Position + new Vector2(0, 270), new Vector2(60, 60), 200,
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures,
                OldGoldMineGame.resources.hudFont, new List<string>() { "Cart.001", "Cart.002", "Cart.003", "Cart.004" });


            scoreMultiplierLabel = new SpriteText(OldGoldMineGame.resources.hudFont, "Score multiplier: 1.0x",
                new Vector2(device.Viewport.Width / 4f, device.Viewport.Height * 0.875f), SpriteText.TextAnchor.MiddleLeft);
        }


        public override void Update()
        {
            speedToggle.Update();
            difficultyToggle.Update();
            seedBox.Update();
            cartSelector.Update();
            startButton.Update();
            backButton.Update();

            scoreMultiplier = 1f + (difficultyToggle.SelectedValueIndex - 1) * 0.5f 
                + speedToggle.SelectedValueIndex * 0.05f;

            scoreMultiplierLabel.Text = scoreMultiplier.ToString("Score multiplier: 0.00#x"); 

            if (startButton.IsClicked())
                OldGoldMineGame.Application.StartGame();
            else if (backButton.IsClicked())
                OldGoldMineGame.Application.ToMainMenu();
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.Black);

            spriteBatch.Begin();

            if (menuBackground != null)
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);

            titleText.Draw(spriteBatch);

            speedLabel.Draw(spriteBatch);

            speedToggle.Draw(spriteBatch);

            difficultyLabel.Draw(spriteBatch);

            difficultyToggle.Draw(spriteBatch);

            seedLabel.Draw(spriteBatch);

            seedBox.Draw(spriteBatch);

            cartPanel.Draw(spriteBatch);

            cartSelector.Draw(spriteBatch);

            scoreMultiplierLabel.Draw(spriteBatch);

            startButton.Draw(spriteBatch);
            backButton.Draw(spriteBatch);

            spriteBatch.End();
        }


        public override void Show()
        {
            return;
        }
    }
}
