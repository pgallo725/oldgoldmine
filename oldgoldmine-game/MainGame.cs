using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using oldgoldmine_game.Menus;
using oldgoldmine_game.Engine;
using oldgoldmine_game.Gameplay;


namespace oldgoldmine_game
{
    public struct GameResources
    {
        /* 3D models for the game */

        public Model m3d_woodenCrate;
        public Model m3d_gold;
        public Model m3d_cart;
        public Model m3d_rails;


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
                this.highlighted = highlighted != null ? highlighted : normal;
                this.pressed = pressed != null ? pressed : normal;
                this.disabled = disabled != null ? disabled : normal;
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
            public int seed;
            public int cart;


            public GameSettings(float multiplier, int startSpeed, int difficulty, int seed, int cart) 
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
        bool freeMovement = false;

        private float currentSpeed = 20f;
        public float Speed { get { return currentSpeed; } set { currentSpeed = value; } }


        const float speedIncreaseInterval = 4f;
        const float maxSpeed = 200f;
        private float lastSpeedUpdate = 0f;
        

        public OldGoldMineGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 4);
            OldGoldMineGame.graphics.SynchronizeWithVerticalRetrace = false;
            OldGoldMineGame.application = this;

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            //graphics.IsFullScreen = true;             // TODO: enable fullscreen support

            Window.Title = "The Old Gold Mine (Alpha)";
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
            

            // Instantiate the procedural generator and initialize the level
            level = new ProceduralGenerator(resources.m3d_rails, 20f, resources.m3d_gold, 0.25f,
                resources.m3d_woodenCrate, new Vector3(2f, 2f, 2f), 150f);


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
            resources.m3d_woodenCrate = Content.Load<Model>("models_3d/woodenCrate");
            resources.m3d_gold = Content.Load<Model>("models_3d/goldOre");
            resources.m3d_cart = Content.Load<Model>("models_3d/cart_lowpoly");
            resources.m3d_rails = Content.Load<Model>("models_3d/rails_segment");

            // Load 2D assets for UI elements
            Texture2D menuButtonTextureNormal = Content.Load<Texture2D>("ui_elements_2d/woodButton_normal");
            Texture2D menuButtonTextureHighlighted = Content.Load<Texture2D>("ui_elements_2d/woodButton_highlighted");

            resources.menuButtonTextures = new GameResources.ButtonTexturePack(menuButtonTextureNormal, menuButtonTextureHighlighted);

            Texture2D leftArrowNormal = Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_normal");
            Texture2D leftArrowHighlighted = Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_highlighted");
            Texture2D leftArrowPressed = Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_pressed");
            Texture2D leftArrowDisabled = Content.Load<Texture2D>("ui_elements_2d/button_leftArrow/leftArrow_disabled");

            Texture2D rightArrowNormal = Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_normal");
            Texture2D rightArrowHighlighted = Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_highlighted");
            Texture2D rightArrowPressed = Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_pressed");
            Texture2D rightArrowDisabled = Content.Load<Texture2D>("ui_elements_2d/button_rightArrow/rightArrow_disabled");

            Texture2D plusButtonNormal = Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_normal");
            Texture2D plusButtonHighlighted = Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_highlighted");
            Texture2D plusButtonPressed = Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_pressed");
            Texture2D plusButtonDisabled = Content.Load<Texture2D>("ui_elements_2d/button_plus/plus_disabled");

            Texture2D minusButtonNormal = Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_normal");
            Texture2D minusButtonHighlighted = Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_highlighted");
            Texture2D minusButtonPressed = Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_pressed");
            Texture2D minusButtonDisabled = Content.Load<Texture2D>("ui_elements_2d/button_minus/minus_disabled");

            Texture2D standardButtonNormal = Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_normal");
            Texture2D standardButtonHighlighted = Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_highlighted");
            Texture2D standardButtonPressed = Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_pressed");
            Texture2D standardButtonDisabled = Content.Load<Texture2D>("ui_elements_2d/button_standard/standard_disabled");

            Texture2D textboxNormal = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_normal");
            Texture2D textboxHighlighted = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_highlighted");
            Texture2D textboxDisabled = Content.Load<Texture2D>("ui_elements_2d/textbox/textbox_disabled");

            resources.framedPanelTexture = Content.Load<Texture2D>("ui_elements_2d/panel_framed");
            resources.lockIcon = Content.Load<Texture2D>("ui_elements_2d/lock_icon");


            resources.leftArrowButtonTextures = new GameResources.ButtonTexturePack(
                leftArrowNormal, leftArrowHighlighted, leftArrowPressed, leftArrowDisabled);

            resources.rightArrowButtonTextures = new GameResources.ButtonTexturePack(
                rightArrowNormal, rightArrowHighlighted, rightArrowPressed, rightArrowDisabled);

            resources.plusButtonTextures = new GameResources.ButtonTexturePack(
                plusButtonNormal, plusButtonHighlighted, plusButtonPressed, plusButtonDisabled);

            resources.minusButtonTextures = new GameResources.ButtonTexturePack(
                minusButtonNormal, minusButtonHighlighted, minusButtonPressed, minusButtonDisabled);

            resources.standardButtonTextures = new GameResources.ButtonTexturePack(
                standardButtonNormal, standardButtonHighlighted, standardButtonPressed, standardButtonDisabled);

            resources.textboxTextures = new GameResources.ButtonTexturePack(
                textboxNormal, textboxHighlighted, textboxHighlighted, textboxDisabled);


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
                        freeMovement = true;

                    if (InputManager.FreeLookKeyPressed)
                    {
                        freeLook = !freeLook;
                        player.ResetCameraLook();
                    }

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

                    if (freeMovement)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            player.Move(moveSpeed, player.Camera.Forward);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            player.Move(moveSpeed, player.Camera.Back);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.A))
                        {
                            player.Move(moveSpeed, player.Camera.Left);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.D))
                        {
                            player.Move(moveSpeed, player.Camera.Right);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        {
                            player.Move(moveSpeed, player.Camera.Up);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                        {
                            player.Move(moveSpeed, player.Camera.Down);
                        }
                    }
                    else
                    {
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
                    }

                    if (freeLook || freeMovement)
                    {
                        player.LookUpDown(InputManager.MouseMovementY * lookAroundSpeed, freeMovement);
                        player.LookLeftRight(InputManager.MouseMovementX * lookAroundSpeed, freeMovement);
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
                    GraphicsDevice.Clear(Color.CornflowerBlue);

                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    GraphicsDevice.RasterizerState = new RasterizerState
                    {
                        CullMode = CullMode.None
                    };

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
                    new GameObject3D(resources.m3d_cart, Vector3.Zero, new Vector3(0.8f, 1f, 1.1f), Quaternion.Identity),
                    new Vector3(0f, -2.4f, -0.75f), 1.2f);

                level.InitializeSeed(gameSettings.seed);
                level.Difficulty = gameSettings.difficulty;

                // Show the game HUD
                hud.Show(timer, 60f, Score, Speed);

                gameState = GameState.Running;
                IsMouseVisible = false;
            }
        }

        public void RestartGame()
        {
            if (gameState == GameState.GameOver)
            {
                gameState = GameState.NewGame;
                StartGame(currentGameInfo);
            }
        }

        public void PauseGame()
        {
            if (gameState == GameState.Running)
            {
                pauseMenu.Show();
                gameState = GameState.Paused;
                IsMouseVisible = true;
                // anything else ?
            }
        }

        public void ResumeGame()
        {
            if (gameState == GameState.Paused)
            {
                gameState = GameState.Running;
                IsMouseVisible = false;
                // anything else ?
            }
        }

        public void GameOver()
        {
            if (gameState == GameState.Running)
            {
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
                mainMenu.Show();
                gameState = GameState.MainMenu;
                IsMouseVisible = true;
                // anything else ?
            }
        }


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
