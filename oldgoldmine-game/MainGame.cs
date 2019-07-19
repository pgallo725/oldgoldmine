using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using oldgoldmine_game.Menus;
using oldgoldmine_game.Engine;
using oldgoldmine_game.Gameplay;


namespace oldgoldmine_game
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


        public struct ButtonTexturePack
        {
            public Texture2D normal;
            public Texture2D highlighted;
            public Texture2D pressed;
            public Texture2D disabled;

            /// <summary>
            /// Create a new texture pack to define the look of a Button
            /// </summary>
            /// <param name="normal">Texture of the button used when in the Normal state.</param>
            /// <param name="highlighted">Texture used to replace the normal look of the button when highlighted.</param>
            /// <param name="pressed">Texture used to replace the normal look when the button is pressed.</param>
            /// <param name="disabled">Texture used to replace the normal look when the button is disabled.</param>
            public ButtonTexturePack(Texture2D normal, Texture2D highlighted = null,
                Texture2D pressed = null, Texture2D disabled = null)
            {
                this.normal = normal;
                this.highlighted = highlighted ?? normal;
                this.pressed = pressed ?? normal;
                this.disabled = disabled ?? normal;
            }

            // 0: Normal, 1: Highlighted, 2: Pressed, 3: Disabled
            public Texture2D this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return this.normal;
                        case 1: return this.highlighted;
                        case 2: return this.pressed;
                        case 3: return this.disabled;
                        default: throw new System.IndexOutOfRangeException();
                    }
                }

                set
                {
                    switch (i)
                    {
                        case 0: this.normal = value; break;
                        case 1: this.highlighted = value; break;
                        case 2: this.pressed = value; break;
                        case 3: this.disabled = value; break;
                        default: throw new System.IndexOutOfRangeException();
                    }
                }
            }
        }


        /* UI elements texture packs */

        public ButtonTexturePack menuButtonTextures;
        public ButtonTexturePack standardButtonTextures;
        public ButtonTexturePack leftArrowButtonTextures;
        public ButtonTexturePack rightArrowButtonTextures;
        public ButtonTexturePack plusButtonTextures;
        public ButtonTexturePack minusButtonTextures;
        public ButtonTexturePack textboxTextures;

        public Texture2D[] cartPreviewImages;

        public Texture2D framedPanelTexture;
        public Texture2D lockIcon;


        /* Fonts */

        public SpriteFont menuButtonFont;
        public SpriteFont settingSelectorFont;
        public SpriteFont menuTitleFont;
        public SpriteFont hudFont;
        public SpriteFont debugInfoFont;
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OldGoldMineGame : Game
    {
        public enum GameState
        {
            MainMenu,
            NewGame,
            Running,
            Paused,
            GameOver
        }

        // Statically accessible graphics and rendering tools
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static BasicEffect basicEffect;


        private static OldGoldMineGame application;
        public static OldGoldMineGame Application { get { return application; } }

        public static GameResources resources = new GameResources();

        GameState gameState;

        HUD hud = new HUD();
        MainMenu mainMenu = new MainMenu();
        CustomizationMenu customMenu = new CustomizationMenu();
        PauseMenu pauseMenu = new PauseMenu();
        GameOverMenu deathMenu = new GameOverMenu();


        public static Timer timer;
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

        bool freeLook = false;

        private float currentSpeed = 20f;
        public float Speed { get { return currentSpeed; } set { currentSpeed = value; } }


        const float speedIncreaseInterval = 4f;
        const float maxSpeed = 200f;
        private float lastSpeedUpdate = 0f;
        

        public OldGoldMineGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 4);
            OldGoldMineGame.graphics.SynchronizeWithVerticalRetrace = false;
            OldGoldMineGame.application = this;

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            //graphics.IsFullScreen = true;             // TODO: enable fullscreen support

            Window.Title = "The Old Gold Mine (Beta)";
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

            BestScore = LoadScore();


            // Initialize menus
            mainMenu.Initialize(GraphicsDevice.Viewport, null);
            customMenu.Initialize(GraphicsDevice.Viewport, null);
            pauseMenu.Initialize(GraphicsDevice.Viewport, null);
            deathMenu.Initialize(GraphicsDevice.Viewport, null);
            

            // Setup HUD
            timer = new Timer();
            hud.Initialize(Window);


            float popupDistance = 400f;

            // Set-up the properties of the GameObjects that will be used in the level generation
                       
            Collectible gold = new Collectible(resources.m3d_gold, Vector3.Zero, 0.3f * Vector3.One, Quaternion.Identity);
            gold.SetFogEffectEnabled(true, Color.Black, popupDistance - 15f, popupDistance);
            gold.SetEmissiveColor(Color.Yellow);

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


            AudioManager.MusicVolume = 0.8f;
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
            AudioManager.AddSong("Cave_AmbientSound", Content.Load<Song>("sounds/Music/Cave_Ambient_Sound"));
            AudioManager.AddSoundEffect("Gold_Pickup", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_GoldPickup"));
            AudioManager.AddSoundEffect("Crash_Sound", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_CrashSounds"));
            AudioManager.AddSoundEffect("Rails_Hit", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_RailsMetalHit"));
            AudioManager.AddSoundEffect("Minecart_Loop", Content.Load<SoundEffect>("sounds/SoundEffects/SFX_MinecartLoop"));

            // Load 2D assets for UI elements

            resources.menuButtonTextures = new GameResources.ButtonTexturePack(
                Content.Load<Texture2D>("ui_elements_2d/woodButton_normal"),
                Content.Load<Texture2D>("ui_elements_2d/woodButton_highlighted"));

            resources.leftArrowButtonTextures = new GameResources.ButtonTexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_disabled"));

            resources.rightArrowButtonTextures = new GameResources.ButtonTexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_disabled"));

            resources.plusButtonTextures = new GameResources.ButtonTexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_disabled"));

            resources.minusButtonTextures = new GameResources.ButtonTexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_disabled"));

            resources.standardButtonTextures = new GameResources.ButtonTexturePack(
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_normal"),
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_highlighted"),
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_pressed"),
                Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_disabled"));


            Texture2D textboxNormal = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_normal");
            Texture2D textboxHighlighted = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_highlighted");
            Texture2D textboxDisabled = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_disabled");

            resources.textboxTextures = new GameResources.ButtonTexturePack(
                textboxNormal, textboxHighlighted, textboxHighlighted, textboxDisabled);


            resources.cartPreviewImages = new Texture2D[4]
            {
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_wooden"),
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_iron"),
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_tank"),
                Content.Load<Texture2D>("ui_elements_2d/preview/cart_preview_golden")
            };


            resources.framedPanelTexture = Content.Load<Texture2D>("ui_elements_2d/panel_framed");
            resources.lockIcon = Content.Load<Texture2D>("ui_elements_2d/lock_icon");

            resources.menuButtonFont = Content.Load<SpriteFont>("ui_elements_2d/MenuFont");
            resources.debugInfoFont = Content.Load<SpriteFont>("ui_elements_2d/SmallFont");
            
            resources.hudFont = resources.menuButtonFont;       // tmp
            resources.menuTitleFont = resources.menuButtonFont;
            resources.settingSelectorFont = resources.hudFont;
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

            switch (gameState)
            {
                case GameState.MainMenu:
                {
                    mainMenu.Update();
                    break;
                }

                case GameState.NewGame:
                {
                    customMenu.Update();
                    break;
                }

                case GameState.Running:
                {
                    if (InputManager.PauseKeyPressed)
                        PauseGame();

                    if (InputManager.DebugKeyPressed)
                    {
                        Collectible.debugDrawHitbox = !Collectible.debugDrawHitbox;
                        Obstacle.debugDrawHitbox = !Obstacle.debugDrawHitbox;
                        hud.ToggleFramerateVisible();
                    }
                        
                    if (InputManager.FreeLookKeyPressed)
                    {
                        freeLook = !freeLook;
                        player.ResetCameraLook();
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

                    float moveSpeed = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    float lookAroundSpeed = 30f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Move the player according to the current frame inputs
                    
                    player.Move(moveSpeed, Vector3.Backward);

                    if (InputManager.RightKeyHold)
                    {
                        player.UpdateSideMovement(gameTime, Vector3.Right);
                    }
                    else if (InputManager.RightKeyReleased)
                    {
                        player.ReverseSideMovement(Vector3.Left);
                    }

                    if (InputManager.LeftKeyHold)
                    {
                        player.UpdateSideMovement(gameTime, Vector3.Left);
                    }
                    else if (InputManager.LeftKeyReleased)
                    {
                        player.ReverseSideMovement(Vector3.Right);
                    }

                    if (InputManager.DownKeyHold)
                    {
                        player.UpdateCrouchMovement(gameTime);
                    }
                    else if (InputManager.DownKeyReleased)
                    {
                        player.ReverseCrouch();
                    }

                    if (InputManager.JumpKeyPressed)
                    {
                        player.Jump();
                    }

                    if (freeLook)
                    {
                        player.LookUpDown(InputManager.MouseMovementY * lookAroundSpeed);
                        player.LookLeftRight(InputManager.MouseMovementX * lookAroundSpeed);
                    }
                    

                    player.Update(gameTime);
                        
                    level.Update(player.Position);

                    break;
                }

                case GameState.Paused:
                {
                    pauseMenu.Update();
                    break;
                }

                case GameState.GameOver:
                {
                    deathMenu.Update();
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

                case GameState.NewGame:
                {
                    customMenu.Draw(GraphicsDevice, spriteBatch);
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
                    deathMenu.Draw(GraphicsDevice, spriteBatch);
                    break;
                }

                default:
                    break;
            }

            base.Draw(gameTime);
        }


        // Methods for managing game status changes

        public void NewGame()
        {
            if (gameState == GameState.MainMenu)
            {
                customMenu.Show();
                gameState = GameState.NewGame;
                IsMouseVisible = true;
            }
        }

        public void StartGame(GameSettings gameSettings)
        {
            if (gameState == GameState.NewGame)
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
                    new Vector3(0f, -2.4f, -0.75f), 1.2f);

                level.InitializeSeed(gameSettings.seed);
                level.Difficulty = gameSettings.difficulty;

                // Show the game HUD
                hud.Show(timer, 60f, Score, Speed);

                player.Start();
                gameState = GameState.Running;
                IsMouseVisible = false;

                // Start playing ambient music
                AudioManager.PlaySong("Cave_AmbientSound", true);
            }
        }

        public void RestartGame()
        {
            if (gameState == GameState.GameOver)
            {
                AudioManager.StopAllEffects();
                gameState = GameState.NewGame;
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
                deathMenu.Show();
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
                if (gameState != GameState.NewGame)
                    AudioManager.PlaySong("Cave_MainTheme");

                AudioManager.StopAllEffects();      // Immediately stop all sound effects being played

                mainMenu.Show();
                gameState = GameState.MainMenu;
                IsMouseVisible = true;
            }
        }


        // Score handling methods

        public static void UpdateScore(int points)
        {
            Score += (int)(points * scoreMultiplier + 0.5f);    // extra 0.5f added to avoid int conversion errors
            OldGoldMineGame.application.hud.UpdateScore(score);
        }

        public void SaveScore(int score)
        {
            const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game";
            Registry.SetValue(key, "Best_Score", score);
        }

        public int LoadScore()
        {
            const string key = "HKEY_CURRENT_USER\\Software\\OldGoldMine\\Game";
            int? score = 0;

            try
            {
                score = (int)Registry.GetValue(key, "Best_Score", 0);   // type int? is nullable (if key doesn't exist)
                if (score == null)
                    score = 0;
            }
            catch (System.Exception)
            {
                // nothing
            }

            return score.Value;
        }
    }
}
