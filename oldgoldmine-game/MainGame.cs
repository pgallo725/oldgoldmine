using System.Linq;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using OldGoldMine.UI;
using OldGoldMine.Menus;
using OldGoldMine.Engine;
using OldGoldMine.Gameplay;


namespace OldGoldMine
{
    public struct GameResources
    {
        /* 3D models for the game */

        public Model[] m3d_carts;
        public Model m3d_gold;
        public Model m3d_cave;
        public Model m3d_lowerObstacle;
        public Model m3d_leftObstacle;
        public Model m3d_rightObstacle;
        public Model m3d_upperObstacle;


        /* UI elements texture packs */

        public Button.TexturePack menuButtonTextures;
        public Button.TexturePack standardButtonTextures;
        public Button.TexturePack leftArrowButtonTextures;
        public Button.TexturePack rightArrowButtonTextures;
        public Button.TexturePack plusButtonTextures;
        public Button.TexturePack minusButtonTextures;
        public Button.TexturePack textboxTextures;


        /* Other UI images and textures */

        public Texture2D mainMenuBackground;
        public Texture2D pauseMenuBackground;
        public Texture2D deathMenuBackground;

        public Texture2D[] cartPreviewImages;

        public Texture2D framedPanelTexture;
        public Texture2D lockIcon;


        /* Fonts */

        public SpriteFont gameTitleFont;
        public SpriteFont menuTitleFont;
        public SpriteFont menuItemsFont;
        public SpriteFont menuSmallFont;
        public SpriteFont hudFont;
        public SpriteFont debugInfoFont;
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OldGoldMineGame : Game
    {
        public struct Settings
        {
            /* Audio settings */

            public int MasterVolume { get; set; }
            public int MusicVolume { get; set; }
            public int EffectsVolume { get; set; }


            /* Video settings */

            public enum DisplayMode
            {
                Fullscreen,
                Windowed,
                Borderless
            }

            public DisplayMode CurrentDisplayMode { get; set; }
            public int ResolutionSetting { get; set; }


            /* Methods */

            public void Load()
            {
                const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game\\Settings";

                int defaultResolution = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
                    .Select((v, i) => new {value = v, index = i})
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
                    settings.MasterVolume = 100;
                    settings.MusicVolume = 100;
                    settings.EffectsVolume = 100;
                    settings.CurrentDisplayMode = DisplayMode.Fullscreen;
                    settings.ResolutionSetting = defaultResolution;
                }
            }

            public void Save()
            {
                const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game\\Settings";
                Registry.SetValue(key, "MasterVolume", settings.MasterVolume);
                Registry.SetValue(key, "MusicVolume", settings.MusicVolume);
                Registry.SetValue(key, "EffectsVolume", settings.EffectsVolume);
                Registry.SetValue(key, "DisplayMode", (int)settings.CurrentDisplayMode);
                Registry.SetValue(key, "ResolutionSetting", settings.ResolutionSetting);
            }

            public void Apply()
            {
                AudioManager.SetVolume(MasterVolume, MusicVolume, EffectsVolume);

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
                graphics.ApplyChanges();

                if (CurrentDisplayMode != DisplayMode.Fullscreen)
                {
                    var screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    var screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    var windowWidth = Application.Window.ClientBounds.Width;
                    var windowHeight = Application.Window.ClientBounds.Height;

                    Application.Window.IsBorderless = (CurrentDisplayMode == DisplayMode.Borderless);
                    Application.Window.Position = new Point((screenWidth - windowWidth) / 2, (screenHeight - windowHeight) / 2);
                }
            }
        }

        public enum GameState
        {
            MainMenu,
            Running,
            Paused,
            GameOver
        }

        // Globally accessible graphics and rendering tools
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static BasicEffect basicEffect;

        public static OldGoldMineGame Application { get; private set; }

        public static Settings settings = new Settings();
        public static GameResources resources = new GameResources();
        
        GameState gameState;

        private HUD hud;
        private MainMenu mainMenu;
        private PauseMenu pauseMenu;
        private GameOverMenu gameOverMenu;


        Timer timer;
        public static Player player;
        ProceduralGenerator level;


        public struct GameSettings
        {
            public float multiplier;
            public float startSpeed;
            public int difficulty;
            public long seed;
            public int cart;


            public GameSettings(float multiplier, int startSpeed, int difficulty, long seed, int cart) 
                : this()
            {
                this.multiplier = multiplier;
                this.startSpeed = startSpeed;
                this.difficulty = difficulty;
                this.seed = seed;
                this.cart = cart;
            }
        }

        private GameSettings currentGameInfo;

        private static float scoreMultiplier = 1f;
        private static int score = 0;
        public static int Score { get { return score; } set { score = value; } }
        private static int bestScore = 0;
        public static int BestScore { get { return bestScore; } set { bestScore = value; } }

        private float currentSpeed = 20f;
        public float Speed { get { return currentSpeed; } set { currentSpeed = value; } }

        const float speedIncreaseInterval = 4f;
        const float maxSpeed = 200f;
        private float lastSpeedUpdate = 0f;
        

        public OldGoldMineGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = false
            };

            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 4);    // 250 FPS target

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = false;

            Window.IsBorderless = false;
            Window.Title = "The Old Gold Mine";

            Application = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            settings.Load();
            settings.Apply();

            BestScore = LoadScore();


            // Initialize menus
            mainMenu = new MainMenu(GraphicsDevice.Viewport, resources.mainMenuBackground);
            pauseMenu = new PauseMenu(GraphicsDevice.Viewport, resources.pauseMenuBackground);
            gameOverMenu = new GameOverMenu(GraphicsDevice.Viewport, resources.deathMenuBackground);
            

            // Setup HUD
            timer = new Timer();
            hud = new HUD(Window);


            float popupDistance = 400f;

            // Set-up the properties of the GameObjects that will be used in the level generation
                       
            Collectible gold = new Collectible(resources.m3d_gold, Vector3.Zero, 0.3f * Vector3.One, Quaternion.Identity);
            gold.SetFogEffectEnabled(true, Color.Black, popupDistance - 15f, popupDistance);
            gold.SetEmissiveColor(Color.Gold);

            Obstacle lowerObstacle = new Obstacle(new GameObject3D(resources.m3d_lowerObstacle,
                Vector3.Zero, 1.2f * Vector3.One, Quaternion.Identity), 
                new BoundingBox(new Vector3(-2f, -1f, -1.2f), new Vector3(2f, 2.5f, 1.2f)));
            lowerObstacle.SetFogEffectEnabled(true, Color.Black, popupDistance - 15f, popupDistance - 5f);

            Obstacle leftObstacle = new Obstacle(new GameObject3D(resources.m3d_leftObstacle),
                new BoundingBox(new Vector3(0f, 0f, -1.5f), new Vector3(4f, 6f, 1.5f)));
            leftObstacle.SetFogEffectEnabled(true, Color.Black, popupDistance - 15f, popupDistance - 5f);

            Obstacle rightObstacle = new Obstacle(new GameObject3D(resources.m3d_rightObstacle),
                new BoundingBox(new Vector3(-4f, 0f, -1.5f), new Vector3(0f, 6f, 1.5f)));
            rightObstacle.SetFogEffectEnabled(true, Color.Black, popupDistance - 15f, popupDistance - 5f);

            Obstacle upperObstacle = new Obstacle(new GameObject3D(resources.m3d_upperObstacle,
                new Vector3(0f, -1.1f, 0f), Vector3.One, Quaternion.Identity),
                new BoundingBox(new Vector3(-3f, 2.75f, -1.2f), new Vector3(3f, 7f, 1.2f)));
            upperObstacle.SetFogEffectEnabled(true, Color.Black, popupDistance - 15f, popupDistance - 5f);

            GameObject3D cave = new GameObject3D(resources.m3d_cave);
            cave.SetFogEffectEnabled(true, Color.Black, popupDistance - 15f, popupDistance - 5f);


            // Instantiate the procedural generator and initialize the level
            level = new ProceduralGenerator(cave, 220f, gold, lowerObstacle,
                leftObstacle, rightObstacle, upperObstacle, popupDistance - 5f);


            AudioManager.PlaySong("Cave_MainTheme", true);

            gameState = GameState.MainMenu;
            IsMouseVisible = true;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // BasicEffect for rendering out primitives
            basicEffect = new BasicEffect(GraphicsDevice)
            {
                Alpha = 1f,

                VertexColorEnabled = true,
                LightingEnabled = false
            };


            // Load 3D models for the game

            resources.m3d_carts = new Model[4] 
            {
                Content.Load<Model>("models_3d/Minecarts/cart_wooden"),
                Content.Load<Model>("models_3d/Minecarts/cart_iron"),
                Content.Load<Model>("models_3d/Minecarts/cart_tank"),
                Content.Load<Model>("models_3d/Minecarts/cart_golden")
            };
            resources.m3d_gold = Content.Load<Model>("models_3d/GoldCollectible/goldOre");
            resources.m3d_cave = Content.Load<Model>("models_3d/Cave/cave_segment");
            resources.m3d_lowerObstacle = Content.Load<Model>("models_3d/ObstacleBottom/obstacle_debris");
            resources.m3d_leftObstacle = Content.Load<Model>("models_3d/ObstacleLeft/obstacle_left");
            resources.m3d_rightObstacle = Content.Load<Model>("models_3d/ObstacleRight/obstacle_right");
            resources.m3d_upperObstacle = Content.Load<Model>("models_3d/ObstacleTop/obstacle_top");

            // Load sound effects and music for AudioManager

            AudioManager.AddSong("Cave_MainTheme", Content.Load<Song>("sounds/Music/Main_Cave_Theme"));
            AudioManager.AddSoundEffect("Cave_Ambient", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_CaveAmbient"));
            AudioManager.AddSoundEffect("Gold_Pickup", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_GoldPickup"));
            AudioManager.AddSoundEffect("Crash_Sound", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_CrashSounds"));
            AudioManager.AddSoundEffect("Rails_Hit", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_RailsMetalHit"));
            AudioManager.AddSoundEffect("Minecart_Loop", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_MinecartLoop"));

            // Load 2D assets for UI elements

            resources.menuButtonTextures = new Button.TexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_main/woodButton_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_main/woodButton_highlighted"));

            resources.leftArrowButtonTextures = new Button.TexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_disabled"));

            resources.rightArrowButtonTextures = new Button.TexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_disabled"));

            resources.plusButtonTextures = new Button.TexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_disabled"));

            resources.minusButtonTextures = new Button.TexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_disabled"));

            resources.standardButtonTextures = new Button.TexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_disabled"));


            Texture2D textboxNormal = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_normal");
            Texture2D textboxHighlighted = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_highlighted");
            Texture2D textboxDisabled = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_disabled");

            resources.textboxTextures = new Button.TexturePack(
                textboxNormal, textboxHighlighted, textboxHighlighted, textboxDisabled);


            resources.cartPreviewImages = new Texture2D[4]
            {
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_wooden"),
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_iron"),
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_tank"),
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_golden")
            };


            resources.framedPanelTexture = Content.Load<Texture2D>("ui_elements_2d/panel/panel_framed");
            resources.lockIcon = Content.Load<Texture2D>("ui_elements_2d/lock_icon");

            resources.mainMenuBackground = Content.Load<Texture2D>("ui_elements_2d/background_images/scene_render_01");
            resources.pauseMenuBackground = Content.Load<Texture2D>("ui_elements_2d/background_images/scene_render_02");
            resources.deathMenuBackground = Content.Load<Texture2D>("ui_elements_2d/background_images/scene_render_03");


            // Load fonts

            resources.gameTitleFont = Content.Load<SpriteFont>("fonts/MainGame_Title_Font");
            resources.menuTitleFont = Content.Load<SpriteFont>("fonts/MenuTitle_Bahnschrift_Font");
            resources.menuItemsFont = Content.Load<SpriteFont>("fonts/MenuItem_Bahnschrift_Font");
            resources.menuSmallFont = Content.Load<SpriteFont>("fonts/MenuSmall_Bahnschrift_Font");

            resources.hudFont = resources.menuItemsFont;
            resources.debugInfoFont = Content.Load<SpriteFont>("fonts/DebugInfo_Font");
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            InputManager.UpdateFrameInput();

            AudioManager.Update(gameTime);

            switch (gameState)
            {
                case GameState.MainMenu:
                {
                    mainMenu.Update();
                    break;
                }

                case GameState.Running:
                {
                    if (InputManager.PausePressed)
                    {
                        PauseGame();
                    }
                        
                    if (InputManager.DebugPressed)
                    {
                        Collectible.debugDrawHitbox = !Collectible.debugDrawHitbox;
                        Obstacle.debugDrawHitbox = !Obstacle.debugDrawHitbox;
                        hud.ToggleFramerateVisible();
                    }

                    // Update time-related information and parameters

                    timer.Update(gameTime);

                    if (timer.time.TotalSeconds >= lastSpeedUpdate + speedIncreaseInterval)
                    {
                        Speed = MathHelper.Clamp(Speed + 1f, 0f, maxSpeed);
                        lastSpeedUpdate = (float)timer.time.TotalSeconds;
                        hud.UpdateSpeed(Speed);
                    }

                    hud.UpdateTimer(timer);
                    hud.UpdateFramerate(1 / gameTime.ElapsedGameTime.TotalSeconds);
                    hud.Show(Window);

                    // Update the player and level status in the current frame

                    player.Update(gameTime);
                    level.Update(gameTime, player.Position);

                    break;
                }

                case GameState.Paused:
                {
                    pauseMenu.Update();
                    break;
                }

                case GameState.GameOver:
                {
                    gameOverMenu.Update();
                    break;
                }

                default:
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                {
                    mainMenu.Draw(GraphicsDevice, spriteBatch);
                    break;
                }

                case GameState.Running:
                {
                    GraphicsDevice.Clear(Color.Black);

                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    // Disable backface culling to avoid artifacts inside the player's cart
                    GraphicsDevice.RasterizerState = new RasterizerState
                    {
                        CullMode = CullMode.None
                    };

                    // Enable anisotropic texture filtering
                    GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

                    // Draw the player and the 3D level (according to the player's POV)

                    player.Draw();
                    level.Draw(player.Camera);

                    // Draw the HUD on top of the rendered scene
                    hud.Draw(spriteBatch);

                    break;
                }

                case GameState.Paused:
                {
                    pauseMenu.Draw(GraphicsDevice, spriteBatch);
                    break;
                }

                case GameState.GameOver:
                {
                    gameOverMenu.Draw(GraphicsDevice, spriteBatch);
                    break;
                }

                default:
                    break;
            }

            base.Draw(gameTime);
        }


        // Methods for managing game status changes

        public void StartGame(GameSettings gameSettings)
        {
            if (gameState == GameState.MainMenu || gameState == GameState.GameOver)
            {
                // Store the new game's settings
                currentGameInfo = gameSettings;

                // Reset the level information
                Speed = gameSettings.startSpeed;
                Score = 0;
                scoreMultiplier = gameSettings.multiplier;
                lastSpeedUpdate = 0f;
                timer.Reset();
                level.Reset();

                // Initialize Camera and Player objects
                GameCamera camera = (player != null) ? player.Camera : new GameCamera();
                camera.Initialize(new Vector3(0f, 2.5f, -15f), Vector3.Zero, GraphicsDevice.DisplayMode.AspectRatio);
                player = new Player(camera,
                    new GameObject3D(resources.m3d_carts[gameSettings.cart], Vector3.Zero, new Vector3(0.8f, 1f, 1.1f), Quaternion.Identity),
                    new Vector3(0f, -2.4f, -0.75f), 1.2f, new Vector3(0f, -0.5f, 0f));

                level.InitializeSeed(gameSettings.seed);
                level.Difficulty = gameSettings.difficulty;

                // Reset the game HUD
                hud.UpdateTimer(timer);
                hud.UpdateScore(Score);
                hud.UpdateSpeed(Speed);

                player.Start();
                gameState = GameState.Running;
                IsMouseVisible = false;

                // Fade-out the menu music
                AudioManager.FadeOutMusic(1.5f);

                // Start playing ambient sounds
                AudioManager.PlaySoundEffect("Cave_Ambient", true, 0.9f);
            }
        }

        public void RestartGame()
        {
            if (gameState == GameState.GameOver)
            {
                AudioManager.StopAllSoundEffects();
                StartGame(currentGameInfo);
            }
        }

        public void PauseGame()
        {
            if (gameState == GameState.Running)
            {
                player.Pause();
                pauseMenu.Show();
                gameState = GameState.Paused;
                IsMouseVisible = true;
            }
        }

        public void ResumeGame()
        {
            if (gameState == GameState.Paused)
            {
                player.Resume();
                gameState = GameState.Running;
                IsMouseVisible = false;
            }
        }

        public void GameOver()
        {
            if (gameState == GameState.Running)
            {
                player.Kill();
                gameOverMenu.Show();
                gameState = GameState.GameOver;
                IsMouseVisible = true;

                if (Score > BestScore)
                {
                    BestScore = Score;
                    SaveScore(Score);
                } 
            }
        }

        public void ToMainMenu()
        {
            if (gameState != GameState.MainMenu)
            {
                AudioManager.PlaySong("Cave_MainTheme", true);

                AudioManager.StopAllSoundEffects();      // Immediately stop all sound effects being played

                mainMenu.Show();
                gameState = GameState.MainMenu;
                IsMouseVisible = true;
            }
        }


        // Score handling methods

        public static void UpdateScore(int points)
        {
            Score += (int)(points * scoreMultiplier + 0.5f);    // extra 0.5f added to avoid int conversion errors
            OldGoldMineGame.Application.hud.UpdateScore(score);
        }

        public void SaveScore(int score)
        {
            const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game";
            Registry.SetValue(key, "Best_Score", score);
        }

        public int LoadScore()
        {
            const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game";
            int? score;

            try
            {
                score = Registry.GetValue(key, "Highscore", 0) as int?;   // type int? is nullable (if key doesn't exist)
                if (score == null)
                    score = 0;
            }
            catch (System.Exception)
            {
                return 0;
            }

            return score.Value;
        }

    }
}
