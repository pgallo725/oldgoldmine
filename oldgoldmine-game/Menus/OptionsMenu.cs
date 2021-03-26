using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    class OptionsMenu : Menu
    {
        private SpriteText menuTitle;

        private SpriteText masterVolumeLabel;
        private ToggleSelector masterVolumeSelector;

        private SpriteText musicVolumeLabel;
        private ToggleSelector musicVolumeSelector;

        private SpriteText effectsVolumeLabel;
        private ToggleSelector effectsVolumeSelector;

        private SpriteText displayModeLabel;
        private ToggleSelector displayModeSelector;

        private SpriteText resolutionLabel;
        private ToggleSelector resolutionSelector;

        private Button cancelButton;
        private Button confirmButton;

        private Vector2 anchorPointVolumes;
        private Vector2 anchorPointDisplay;
        private Vector2 anchorPointButtons;


        public override void Initialize(Viewport viewport, Texture2D background, Menu parent)
        {
            this.parent = parent;
            this.menuBackground = background;
            this.semiTransparencyLayer = new Image(new SolidColorTexture(OldGoldMineGame.graphics.GraphicsDevice,
                new Color(Color.Black, 0.66f)), viewport.Bounds.Center.ToVector2(), viewport.Bounds.Size.ToVector2());

            Vector2 buttonSize = new Vector2(225, 75);

            float screenWidth = viewport.Width;
            float screenHeight = viewport.Height;

            anchorPointVolumes = new Vector2(screenWidth * 0.50f, screenHeight * 0.25f);        // Position at centered X and -25% Y from the center
            anchorPointDisplay = new Vector2(screenWidth * 0.50f, screenHeight * 0.625f);       // Position at centered X and +12.5% Y from the center
            anchorPointButtons = new Vector2(screenWidth * 0.50f, screenHeight * 0.89f);        // Position at +25% X and +39% Y from the center


            // OPTIONS MENU LAYOUT SETUP

            menuTitle = new SpriteText(OldGoldMineGame.resources.menuTitleFont, "EDIT SETTINGS",
                Color.LightGray, new Vector2(viewport.Width / 2, viewport.Height * 0.12f), SpriteText.TextAnchor.MiddleCenter);


            masterVolumeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Master volume",
                anchorPointVolumes - new Vector2(300, 0), SpriteText.TextAnchor.MiddleCenter);

            masterVolumeSelector = new ToggleSelector(masterVolumeLabel.Position + new Vector2(600, 0), new Vector2(75, 75), 200,
                OldGoldMineGame.resources.menuItemsFont, new List<string>() { "0", "5", "10", "15", "20", "25", "30", "35", "40",
                "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" },
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures, Color.BurlyWood);

            musicVolumeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Music volume",
                masterVolumeLabel.Position + new Vector2(0, 100), SpriteText.TextAnchor.MiddleCenter);

            musicVolumeSelector = new ToggleSelector(musicVolumeLabel.Position + new Vector2(600, 0), new Vector2(75, 75), 200,
                OldGoldMineGame.resources.menuItemsFont, new List<string>() { "0", "5", "10", "15", "20", "25", "30", "35", "40",
                "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" },
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures, Color.BurlyWood);

            effectsVolumeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "SoundEffects volume",
                musicVolumeLabel.Position + new Vector2(0, 100), SpriteText.TextAnchor.MiddleCenter);

            effectsVolumeSelector = new ToggleSelector(effectsVolumeLabel.Position + new Vector2(600, 0), new Vector2(75, 75), 200,
                OldGoldMineGame.resources.menuItemsFont, new List<string>() { "0", "5", "10", "15", "20", "25", "30", "35", "40",
                "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" },
                OldGoldMineGame.resources.minusButtonTextures, OldGoldMineGame.resources.plusButtonTextures, Color.BurlyWood);


            displayModeLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Display mode",
                anchorPointDisplay - new Vector2(300, 0), SpriteText.TextAnchor.MiddleCenter);

            displayModeSelector = new ToggleSelector(displayModeLabel.Position + new Vector2(600, 0), new Vector2(75, 75), 300,
                OldGoldMineGame.resources.menuItemsFont, new List<string>(Enum.GetNames(typeof(OldGoldMineGame.Settings.DisplayMode))),
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures, Color.BurlyWood);

            resolutionLabel = new SpriteText(OldGoldMineGame.resources.menuItemsFont, "Resolution",
                displayModeLabel.Position + new Vector2(0, 100), SpriteText.TextAnchor.MiddleCenter);

            resolutionSelector = new ToggleSelector(resolutionLabel.Position + new Vector2(600, 0), new Vector2(75, 75), 300,
                OldGoldMineGame.resources.menuItemsFont,
                GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Select(dm => dm.Width + " x " + dm.Height).ToList(),
                OldGoldMineGame.resources.leftArrowButtonTextures, OldGoldMineGame.resources.rightArrowButtonTextures, Color.BurlyWood);


            cancelButton = new Button(anchorPointButtons - new Vector2(buttonSize.X / 2f + 10f, 0), buttonSize,
                OldGoldMineGame.resources.menuItemsFont, "BACK", Color.LightGoldenrodYellow,
                OldGoldMineGame.resources.standardButtonTextures, Color.BurlyWood);

            confirmButton = new Button(anchorPointButtons + new Vector2(buttonSize.X / 2f + 10f, 0), buttonSize,
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
            else if (cancelButton.Update() || InputManager.PauseKeyPressed)
            {
                parent.CloseSubmenu();
                return;
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

            // TODO need to re-initialize all menus when the resolution is changed

            confirmButton.Enabled = false;
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


        public override void Show()
        {
            masterVolumeSelector.SelectedValueIndex = OldGoldMineGame.settings.MasterVolume / 5;
            musicVolumeSelector.SelectedValueIndex = OldGoldMineGame.settings.MusicVolume / 5;
            effectsVolumeSelector.SelectedValueIndex = OldGoldMineGame.settings.EffectsVolume / 5;

            displayModeSelector.SelectedValueIndex = (int)OldGoldMineGame.settings.CurrentDisplayMode;
            resolutionSelector.SelectedValueIndex = OldGoldMineGame.settings.ResolutionSetting;

            confirmButton.Enabled = false;
            cancelButton.Enabled = true;
        }

        public override void CloseSubmenu()
        {
            // OptionsMenu has no nested submenus
            throw new NotSupportedException();
        }
    }
}
