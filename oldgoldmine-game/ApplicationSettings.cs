using System.Linq;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;


namespace OldGoldMine
{
    public static class ApplicationSettings
    {
        const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game\\Settings";

        /* Audio settings */

        /// <summary>
        /// The volume level of the overall application (influences all sounds), on a range from 0 to 100.
        /// </summary>
        public static int MasterVolume { get; set; }

        /// <summary>
        /// The volume level of background music, on a range from 0 to 100.
        /// </summary>
        public static int MusicVolume { get; set; }

        /// <summary>
        /// The volume level of all sound effects played by the game, on a range from 0 to 100.
        /// </summary>
        public static int EffectsVolume { get; set; }


        /* Video settings */

        public enum DisplayMode
        {
            Fullscreen,
            Windowed,
            Borderless
        }

        /// <summary>
        /// The display mode for the application window (Fullscreen, Windowed, Borderless).
        /// </summary>
        public static DisplayMode CurrentDisplayMode { get; set; }

        /// <summary>
        /// The index of the currently selected resolution mode, among those supported by the device.
        /// </summary>
        public static int ResolutionSetting { get; set; }


        /// <summary>
        /// Load values for all settings by reading them from the system registry, otherwise default values are set.
        /// </summary>
        public static void Load()
        {
            int defaultResolution = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
                .Select((v, i) => new { value = v, index = i })
                .First(item => item.value == GraphicsAdapter.DefaultAdapter.CurrentDisplayMode)
                .index;

            try     /* Read settings values from the registry */
            {
                MasterVolume = (int)(Registry.GetValue(key, "MasterVolume", 100) ?? 100);
                MusicVolume = (int)(Registry.GetValue(key, "MusicVolume", 100) ?? 100);
                EffectsVolume = (int)(Registry.GetValue(key, "EffectsVolume", 100) ?? 100);

                CurrentDisplayMode = (DisplayMode)(Registry.GetValue(key, "DisplayMode", 0) ?? 0);
                ResolutionSetting = (int)(Registry.GetValue(key, "ResolutionSetting", defaultResolution) ?? defaultResolution);
            }
            catch (System.Exception)    /* Load default settings */
            {
                MasterVolume = 100;
                MusicVolume = 100;
                EffectsVolume = 100;
                CurrentDisplayMode = DisplayMode.Fullscreen;
                ResolutionSetting = defaultResolution;
            }
        }

        /// <summary>
        /// Write all the currently selected settings to the system registry, in order to save them.
        /// </summary>
        public static void Save()
        {
            Registry.SetValue(key, "MasterVolume", MasterVolume);
            Registry.SetValue(key, "MusicVolume", MusicVolume);
            Registry.SetValue(key, "EffectsVolume", EffectsVolume);
            Registry.SetValue(key, "DisplayMode", (int)CurrentDisplayMode);
            Registry.SetValue(key, "ResolutionSetting", ResolutionSetting);
        }

        /// <summary>
        /// Apply the currently selected settings by sending commands to the AudioManager, GraphicsDevice and GameWindow.
        /// </summary>
        public static void Apply()
        {
            AudioManager.SetVolume(MasterVolume, MusicVolume, EffectsVolume);
            var graphics = OldGoldMineGame.Graphics;

            try
            {
                Microsoft.Xna.Framework.Graphics.DisplayMode resolution =
                    GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
                        .Skip(ResolutionSetting)
                        .Take(1)
                        .Single();

                graphics.PreferredBackBufferWidth = resolution.Width;
                graphics.PreferredBackBufferHeight = resolution.Height;
            }
            catch
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            graphics.IsFullScreen = (CurrentDisplayMode == DisplayMode.Fullscreen);
            graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
            graphics.ApplyChanges();

            if (CurrentDisplayMode != DisplayMode.Fullscreen)
            {
                var screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                var screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                var windowWidth = OldGoldMineGame.Application.Window.ClientBounds.Width;
                var windowHeight = OldGoldMineGame.Application.Window.ClientBounds.Height;

                OldGoldMineGame.Application.Window.IsBorderless = (CurrentDisplayMode == DisplayMode.Borderless);
                OldGoldMineGame.Application.Window.Position = new Point((screenWidth - windowWidth) / 2, (screenHeight - windowHeight) / 2);
            }
        }

    }
}
