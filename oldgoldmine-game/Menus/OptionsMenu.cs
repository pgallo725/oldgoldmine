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
            : base(background, new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.66f)), new Point(225, 75), parent)
        {
            Point anchorPointVolumes = new Point(viewport.Width / 2, (int)(viewport.Height * 0.25f));
            Point anchorPointDisplay = new Point(viewport.Width / 2, (int)(viewport.Height * 0.625f));
            Point anchorPointButtons = new Point(viewport.Width / 2, (int)(viewport.Height * 0.89f));

            List<string> volumeValues = new List<string>() { "0", "5", "10", "15", "20", "25", "30",
                "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" };

            // OPTIONS MENU LAYOUT SETUP

            menuTitle = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "EDIT SETTINGS",
                Color.LightGray, new Point(viewport.Width / 2, (int)(viewport.Height * 0.12f)));


            masterVolumeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Master volume",
                Color.White, anchorPointVolumes - new Point(300, 0));

            masterVolumeSelector = new Selector(masterVolumeLabel.Position + new Point(600, 0), new Point(75, 75), 200,
                OldGoldMineGame.resources.menuItemsFont, volumeValues, Color.White,
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures, Color.BurlyWood);

            musicVolumeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Music volume",
                Color.White, masterVolumeLabel.Position + new Point(0, 100));

            musicVolumeSelector = new Selector(musicVolumeLabel.Position + new Point(600, 0), new Point(75, 75), 200,
                OldGoldMineGame.resources.menuItemsFont, volumeValues, Color.White,
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures, Color.BurlyWood);

            effectsVolumeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "SoundEffects volume", Color.White,
                musicVolumeLabel.Position + new Point(0, 100), SpriteText.TextAnchor.MiddleCenter);

            effectsVolumeSelector = new Selector(effectsVolumeLabel.Position + new Point(600, 0), new Point(75, 75), 200,
                OldGoldMineGame.resources.menuItemsFont, volumeValues, Color.White,
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures, Color.BurlyWood);


            displayModeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Display mode", Color.White,
                anchorPointDisplay - new Point(300, 0), SpriteText.TextAnchor.MiddleCenter);

            displayModeSelector = new Selector(displayModeLabel.Position + new Point(600, 0), new Point(75, 75), 300,
                OldGoldMineGame.resources.menuItemsFont, Enum.GetNames(typeof(OldGoldMineGame.Settings.DisplayMode)).ToList(), Color.White,
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures, Color.BurlyWood);

            resolutionLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Resolution",
                Color.White, displayModeLabel.Position + new Point(0, 100));

            resolutionSelector = new Selector(resolutionLabel.Position + new Point(600, 0), new Point(75, 75), 300,
                OldGoldMineGame.resources.menuItemsFont,
                GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Select(dm => dm.Width + " x " + dm.Height).ToList(), Color.White,
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures, Color.BurlyWood);


            cancelButton = new Button(anchorPointButtons - new Point(buttonSize.X / 2 + 10, 0), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "BACK", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.standardButtonTextures, Color.BurlyWood);

            confirmButton = new Button(anchorPointButtons + new Point(buttonSize.X / 2 + 10, 0), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "APPLY", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.standardButtonTextures, Color.BurlyWood);
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
                OldGoldMineGame.settings.Save();
            }
            else if (cancelButton.Update() || InputManager.PausePressed)
            {
                parent.Show();
            }
        }


        public void ApplySettings()
        {
            OldGoldMineGame.settings.MasterVolume = int.Parse(masterVolumeSelector.SelectedValue);
            OldGoldMineGame.settings.MusicVolume = int.Parse(musicVolumeSelector.SelectedValue);
            OldGoldMineGame.settings.EffectsVolume = int.Parse(effectsVolumeSelector.SelectedValue);

            OldGoldMineGame.settings.CurrentDisplayMode = Enum.Parse<OldGoldMineGame.Settings.DisplayMode>(displayModeSelector.SelectedValue);
            OldGoldMineGame.settings.ResolutionSetting = resolutionSelector.SelectedValueIndex;

            OldGoldMineGame.settings.Apply();

            Layout();   // Update menu layout on-the-fly when the game resolution is changed

            confirmButton.Enabled = false;
        }


        protected override void Layout()
        {
            Viewport viewport = OldGoldMineGame.graphics.GraphicsDevice.Viewport;

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

            masterVolumeSelector.SelectedValueIndex = OldGoldMineGame.settings.MasterVolume / 5;
            musicVolumeSelector.SelectedValueIndex = OldGoldMineGame.settings.MusicVolume / 5;
            effectsVolumeSelector.SelectedValueIndex = OldGoldMineGame.settings.EffectsVolume / 5;

            displayModeSelector.SelectedValueIndex = (int)OldGoldMineGame.settings.CurrentDisplayMode;
            resolutionSelector.SelectedValueIndex = OldGoldMineGame.settings.ResolutionSetting;

            confirmButton.Enabled = false;
            cancelButton.Enabled = true;
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
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
