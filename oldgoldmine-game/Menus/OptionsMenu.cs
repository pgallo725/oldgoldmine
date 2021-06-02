using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;
using OldGoldMine.UI;


namespace OldGoldMine.Menus
{
    class OptionsMenu : Menu
    {
        private readonly SpriteText menuTitle;

        private readonly SpriteText masterVolumeLabel;
        private readonly Selector masterVolumeSelector;

        private readonly SpriteText musicVolumeLabel;
        private readonly Selector musicVolumeSelector;

        private readonly SpriteText effectsVolumeLabel;
        private readonly Selector effectsVolumeSelector;

        private readonly SpriteText displayModeLabel;
        private readonly Selector displayModeSelector;

        private readonly SpriteText resolutionLabel;
        private readonly Selector resolutionSelector;

        private readonly Button cancelButton;
        private readonly Button confirmButton;


        public OptionsMenu(Viewport viewport, Texture2D background, Menu parent)
            : base(background, new SolidColorTexture(OldGoldMineGame.Graphics.GraphicsDevice,
                new Color(Color.Black, 0.66f)), new Point(225, 75), parent)
        {
            Point anchorPointVolumes = new Point(viewport.Width / 2, (int)(viewport.Height * 0.25f));
            Point anchorPointDisplay = new Point(viewport.Width / 2, (int)(viewport.Height * 0.625f));
            Point anchorPointButtons = new Point(viewport.Width / 2, (int)(viewport.Height * 0.89f));

            List<string> volumeValues = new List<string>() { "0", "5", "10", "15", "20", "25", "30",
                "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" };

            // OPTIONS MENU LAYOUT SETUP

            menuTitle = new SpriteText(Resources.GetFont("MenuTitle"), "EDIT SETTINGS",
                Color.LightGray, new Point(viewport.Width / 2, (int)(viewport.Height * 0.12f)));


            masterVolumeLabel = new SpriteText(Resources.GetFont("MenuItem"), "Master volume",
                Color.White, anchorPointVolumes - new Point(300, 0));

            masterVolumeSelector = new Selector(masterVolumeLabel.Position + new Point(600, 0), new Point(75, 75), 200,
                Resources.GetFont("MenuItem"), volumeValues, Color.White,
                Resources.GetSpritePack("MinusButton"), Resources.GetSpritePack("PlusButton"), Color.BurlyWood);

            musicVolumeLabel = new SpriteText(Resources.GetFont("MenuItem"), "Music volume",
                Color.White, masterVolumeLabel.Position + new Point(0, 100));

            musicVolumeSelector = new Selector(musicVolumeLabel.Position + new Point(600, 0), new Point(75, 75), 200,
                Resources.GetFont("MenuItem"), volumeValues, Color.White,
                Resources.GetSpritePack("MinusButton"), Resources.GetSpritePack("PlusButton"), Color.BurlyWood);

            effectsVolumeLabel = new SpriteText(Resources.GetFont("MenuItem"), "SoundEffects volume", Color.White,
                musicVolumeLabel.Position + new Point(0, 100), SpriteText.TextAnchor.MiddleCenter);

            effectsVolumeSelector = new Selector(effectsVolumeLabel.Position + new Point(600, 0), new Point(75, 75), 200,
                Resources.GetFont("MenuItem"), volumeValues, Color.White,
                Resources.GetSpritePack("MinusButton"), Resources.GetSpritePack("PlusButton"), Color.BurlyWood);


            displayModeLabel = new SpriteText(Resources.GetFont("MenuItem"), "Display mode", Color.White,
                anchorPointDisplay - new Point(300, 0), SpriteText.TextAnchor.MiddleCenter);

            displayModeSelector = new Selector(displayModeLabel.Position + new Point(600, 0), new Point(75, 75), 300,
                Resources.GetFont("MenuItem"), Enum.GetNames(typeof(ApplicationSettings.DisplayMode)).ToList(), Color.White,
                Resources.GetSpritePack("LeftArrowButton"), Resources.GetSpritePack("RightArrowButton"), Color.BurlyWood);

            resolutionLabel = new SpriteText(Resources.GetFont("MenuItem"), "Resolution",
                Color.White, displayModeLabel.Position + new Point(0, 100));

            resolutionSelector = new Selector(resolutionLabel.Position + new Point(600, 0), new Point(75, 75), 300,
                Resources.GetFont("MenuItem"),
                GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Select(dm => dm.Width + " x " + dm.Height).ToList(), Color.White,
                Resources.GetSpritePack("LeftArrowButton"), Resources.GetSpritePack("RightArrowButton"), Color.BurlyWood);


            cancelButton = new Button(anchorPointButtons - new Point(buttonSize.X / 2 + 10, 0), buttonSize,
                Resources.GetFont("MenuItem"), "BACK", Color.LightGoldenrodYellow,
                Resources.GetSpritePack("StandardButton"), Color.BurlyWood);

            confirmButton = new Button(anchorPointButtons + new Point(buttonSize.X / 2 + 10, 0), buttonSize,
                Resources.GetFont("MenuItem"), "APPLY", Color.LightGoldenrodYellow,
                Resources.GetSpritePack("StandardButton"), Color.BurlyWood);
        }


        public override void Update()
        {
            bool settingsChanged = false;
            settingsChanged |= masterVolumeSelector.Update();
            settingsChanged |= musicVolumeSelector.Update();
            settingsChanged |= effectsVolumeSelector.Update();
            settingsChanged |= displayModeSelector.Update();
            settingsChanged |= resolutionSelector.Update();

            if (settingsChanged)
            {
                confirmButton.Enabled = true;
            }

            if (confirmButton.Update())
            {
                ApplySettings();
                ApplicationSettings.Save();
            }
            else if (cancelButton.Update() || InputManager.PausePressed)
            {
                parent.Show();
            }
        }


        public void ApplySettings()
        {
            ApplicationSettings.MasterVolume = int.Parse(masterVolumeSelector.SelectedValue);
            ApplicationSettings.MusicVolume = int.Parse(musicVolumeSelector.SelectedValue);
            ApplicationSettings.EffectsVolume = int.Parse(effectsVolumeSelector.SelectedValue);

            ApplicationSettings.CurrentDisplayMode = Enum.Parse<ApplicationSettings.DisplayMode>(displayModeSelector.SelectedValue);
            ApplicationSettings.ResolutionSetting = resolutionSelector.SelectedValueIndex;

            ApplicationSettings.Apply();

            Layout();   // Update menu layout on-the-fly when the game resolution is changed

            confirmButton.Enabled = false;
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.Graphics.GraphicsDevice.Viewport;

            Point anchorPointVolumes = new Point(viewport.Width / 2, (int)(viewport.Height * 0.25f));
            Point anchorPointDisplay = new Point(viewport.Width / 2, (int)(viewport.Height * 0.625f));
            Point anchorPointButtons = new Point(viewport.Width / 2, (int)(viewport.Height * 0.89f));

            menuTitle.Position = new Point(viewport.Width / 2, (int)(viewport.Height * 0.12f));

            masterVolumeLabel.Position = anchorPointVolumes - new Point(300, 0);
            masterVolumeSelector.Position = masterVolumeLabel.Position + new Point(600, 0);

            musicVolumeLabel.Position = masterVolumeLabel.Position + new Point(0, 100);
            musicVolumeSelector.Position = musicVolumeLabel.Position + new Point(600, 0);

            effectsVolumeLabel.Position = musicVolumeLabel.Position + new Point(0, 100);
            effectsVolumeSelector.Position = effectsVolumeLabel.Position + new Point(600, 0);

            displayModeLabel.Position = anchorPointDisplay - new Point(300, 0);
            displayModeSelector.Position = displayModeLabel.Position + new Point(600, 0);

            resolutionLabel.Position = displayModeLabel.Position + new Point(0, 100);
            resolutionSelector.Position = resolutionLabel.Position + new Point(600, 0);

            cancelButton.Position = anchorPointButtons - new Point(buttonSize.X / 2 + 10, 0);
            confirmButton.Position = anchorPointButtons + new Point(buttonSize.X / 2 + 10, 0);
        }


        public override void Show()
        {
            Layout();

            masterVolumeSelector.SelectedValueIndex = ApplicationSettings.MasterVolume / 5;
            musicVolumeSelector.SelectedValueIndex = ApplicationSettings.MusicVolume / 5;
            effectsVolumeSelector.SelectedValueIndex = ApplicationSettings.EffectsVolume / 5;

            displayModeSelector.SelectedValueIndex = (int)ApplicationSettings.CurrentDisplayMode;
            resolutionSelector.SelectedValueIndex = ApplicationSettings.ResolutionSetting;

            confirmButton.Enabled = false;
            cancelButton.Enabled = true;
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

            menuTitle.Draw(spriteBatch);

            masterVolumeLabel.Draw(spriteBatch);
            masterVolumeSelector.Draw(spriteBatch);

            musicVolumeLabel.Draw(spriteBatch);
            musicVolumeSelector.Draw(spriteBatch);

            effectsVolumeLabel.Draw(spriteBatch);
            effectsVolumeSelector.Draw(spriteBatch);

            displayModeLabel.Draw(spriteBatch);
            displayModeSelector.Draw(spriteBatch);

            resolutionLabel.Draw(spriteBatch);
            resolutionSelector.Draw(spriteBatch);

            confirmButton.Draw(spriteBatch);
            cancelButton.Draw(spriteBatch);

            spriteBatch.End();
        }

    }
}
