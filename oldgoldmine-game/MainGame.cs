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
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OldGoldMineGame : Game
    {
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

        public static OldGoldMineGame Application { get; private set; }
        
        GameState gameState;

        private MainMenu mainMenu;
        private PauseMenu pauseMenu;
        private GameOverMenu gameOverMenu;

        Timer timer;
        public static Player player;
        Level level;

        private GameSettings currentGameInfo;

        private float currentSpeed = 20f;
        public float Speed { get { return currentSpeed; } set { currentSpeed = value; } }

        const float speedIncreaseInterval = 4f;
        const float maxSpeed = 200f;
        private float lastSpeedUpdate = 0f;


        public OldGoldMineGame()
        {
            Content.RootDirectory = "Assets";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 4);    // 250 FPS target

            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = false,
                PreferMultiSampling = true,
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
                IsFullScreen = false
            };

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

            ApplicationSettings.Load();
            ApplicationSettings.Apply();

            Score.Load();

            timer = new Timer();

            // Initialize menus and HUD
            mainMenu = new MainMenu(GraphicsDevice.Viewport, Resources.GetTexture("MainBackground"));
            pauseMenu = new PauseMenu(GraphicsDevice.Viewport, Resources.GetTexture("PauseBackground"));
            gameOverMenu = new GameOverMenu(GraphicsDevice.Viewport, Resources.GetTexture("GameOverBackground"));

            HUD.Create(Window);           


            float popupDistance = 400f;

            // Set-up the properties of the GameObjects that will be used in the level generation
                       
            Collectible gold = new Collectible(Resources.GetModel("GoldOre"), 
                Vector3.Zero, 0.33f * Vector3.One, Quaternion.Identity);
            gold.SetFogEffect(true, Color.Black, popupDistance - 15f, popupDistance);
            gold.SetEmissiveColor(Color.Gold * 0.78f);

            Obstacle lowerObstacle = new Obstacle(Resources.GetModel("LowerObstacle"),
                Vector3.Zero, 1.2f * Vector3.One, Quaternion.Identity, 
                new BoundingBox(new Vector3(-2f, -1f, -1.2f), new Vector3(2f, 2.5f, 1.2f)));
            lowerObstacle.SetFogEffect(true, Color.Black, popupDistance - 15f, popupDistance - 5f);

            Obstacle leftObstacle = new Obstacle(Resources.GetModel("LeftObstacle"),
                Vector3.Zero, Vector3.One, Quaternion.Identity,
                new BoundingBox(new Vector3(0f, 0f, -1.5f), new Vector3(4f, 6f, 1.5f)));
            leftObstacle.SetFogEffect(true, Color.Black, popupDistance - 15f, popupDistance - 5f);
            leftObstacle.EnableDefaultLighting();
            leftObstacle.SetSpecularSettings(Color.Transparent, 1f);

            Obstacle rightObstacle = new Obstacle(Resources.GetModel("RightObstacle"),
                Vector3.Zero, Vector3.One, Quaternion.Identity,
                new BoundingBox(new Vector3(-4f, 0f, -1.5f), new Vector3(0f, 6f, 1.5f)));
            rightObstacle.SetFogEffect(true, Color.Black, popupDistance - 15f, popupDistance - 5f);
            rightObstacle.EnableDefaultLighting();
            rightObstacle.SetSpecularSettings(Color.Transparent, 1f);

            Obstacle upperObstacle = new Obstacle(Resources.GetModel("UpperObstacle"),
                new Vector3(0f, -1.1f, 0f), Vector3.One, Quaternion.Identity,
                new BoundingBox(new Vector3(-3f, 2.75f, -1.2f), new Vector3(3f, 7f, 1.2f)));
            upperObstacle.SetFogEffect(true, Color.Black, popupDistance - 15f, popupDistance - 5f);
            //upperObstacle.EnableDefaultLighting();
            //upperObstacle.SetSpecularSettings(Color.Transparent, 1f);


            GameObject3D cave = new GameObject3D(Resources.GetModel("CaveSegment"));
            cave.SetFogEffect(true, Color.Black, popupDistance - 15f, popupDistance - 5f);


            Model[] caveProps = new Model[3]
            {
                Resources.GetModel("CaveProps_0"),
                Resources.GetModel("CaveProps_1"),
                Resources.GetModel("CaveProps_2")
            };

            // Prepare the props models to be used inside the cave
            foreach (Model model in caveProps)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    if (mesh.Name.Contains("LanternEmissive"))
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.FogEnabled = true;
                            effect.FogColor = Color.Black.ToVector3();
                            effect.FogStart = popupDistance - 15f;
                            effect.FogEnd = popupDistance - 5f;

                            effect.EmissiveColor = new Vector3(0.92f, 0.85f, 0.68f);
                        }
                    }
                }
            }

            GameObject3D[] caveObjects = new GameObject3D[3]
            {
                new GameObject3D(caveProps[0]),
                new GameObject3D(caveProps[1]),
                new GameObject3D(caveProps[2])
            };


            // Instantiate the procedural generator and initialize the level
            level = new Level(cave, 220f, caveObjects, gold, lowerObstacle,
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


            // LOAD 3D MODELS

            Resources.AddModel("Cart_0", Content.Load<Model>("Models/Cart/WoodenCart/cart_wooden"));
            Resources.AddModel("Cart_1", Content.Load<Model>("Models/Cart/IronCart/cart_iron"));
            Resources.AddModel("Cart_2", Content.Load<Model>("Models/Cart/AluminumCart/cart_aluminum"));
            Resources.AddModel("Cart_3", Content.Load<Model>("Models/Cart/TankCart/cart_tank"));
            Resources.AddModel("Cart_4", Content.Load<Model>("Models/Cart/GoldenCart/cart_golden"));

            Resources.AddModel("CaveSegment", Content.Load<Model>("Models/Cave/cave_segment"));
            Resources.AddModel("CaveProps_0", Content.Load<Model>("Models/Cave/cave_props_01"));
            Resources.AddModel("CaveProps_1", Content.Load<Model>("Models/Cave/cave_props_02"));
            Resources.AddModel("CaveProps_2", Content.Load<Model>("Models/Cave/cave_props_03"));

            Resources.AddModel("GoldOre", Content.Load<Model>("Models/GoldOre/goldOre"));
            Resources.AddModel("LowerObstacle", Content.Load<Model>("Models/Obstacles/ObstacleBottom/obstacle_bottom"));
            Resources.AddModel("LeftObstacle", Content.Load<Model>("Models/Obstacles/ObstacleLeft/obstacle_left"));
            Resources.AddModel("RightObstacle", Content.Load<Model>("Models/Obstacles/ObstacleRight/obstacle_right"));
            Resources.AddModel("UpperObstacle", Content.Load<Model>("Models/Obstacles/ObstacleTop/obstacle_top"));

            // LOAD SOUND EFFECTS AND MUSIC

            AudioManager.AddSong("Cave_MainTheme", Content.Load<Song>("Sounds/Music/Main_Cave_Theme"));
            AudioManager.AddSoundEffect("Cave_Ambient", Content.Load<SoundEffect>("Sounds/SoundEffects/SFX_CaveAmbient"));
            AudioManager.AddSoundEffect("Gold_Pickup", Content.Load<SoundEffect>("Sounds/SoundEffects/SFX_GoldPickup"));
            AudioManager.AddSoundEffect("Crash_Sound", Content.Load<SoundEffect>("Sounds/SoundEffects/SFX_CrashSounds"));
            AudioManager.AddSoundEffect("Rails_Hit", Content.Load<SoundEffect>("Sounds/SoundEffects/SFX_RailsMetalHit"));
            AudioManager.AddSoundEffect("Minecart_Loop", Content.Load<SoundEffect>("Sounds/SoundEffects/SFX_MinecartLoop"));

            // LOAD 2D ASSETS

            Resources.AddSpritePack("MainButton", new Button.SpritePack(
                Content.Load<Texture2D>("UI/button_main/woodButton_normal"),
                Content.Load<Texture2D>("UI/button_main/woodButton_highlighted")));

            Resources.AddSpritePack("LeftArrowButton", new Button.SpritePack(
                Content.Load<Texture2D>("UI/button_leftArrow/leftArrow_normal"),
                Content.Load<Texture2D>("UI/button_leftArrow/leftArrow_highlighted"),
                Content.Load<Texture2D>("UI/button_leftArrow/leftArrow_pressed"),
                Content.Load<Texture2D>("UI/button_leftArrow/leftArrow_disabled")));

            Resources.AddSpritePack("RightArrowButton", new Button.SpritePack(
                Content.Load<Texture2D>("UI/button_rightArrow/rightArrow_normal"),
                Content.Load<Texture2D>("UI/button_rightArrow/rightArrow_highlighted"),
                Content.Load<Texture2D>("UI/button_rightArrow/rightArrow_pressed"),
                Content.Load<Texture2D>("UI/button_rightArrow/rightArrow_disabled")));

            Resources.AddSpritePack("PlusButton", new Button.SpritePack(
                Content.Load<Texture2D>("UI/button_plus/plus_normal"),
                Content.Load<Texture2D>("UI/button_plus/plus_highlighted"),
                Content.Load<Texture2D>("UI/button_plus/plus_pressed"),
                Content.Load<Texture2D>("UI/button_plus/plus_disabled")));

            Resources.AddSpritePack("MinusButton", new Button.SpritePack(
                Content.Load<Texture2D>("UI/button_minus/minus_normal"),
                Content.Load<Texture2D>("UI/button_minus/minus_highlighted"),
                Content.Load<Texture2D>("UI/button_minus/minus_pressed"),
                Content.Load<Texture2D>("UI/button_minus/minus_disabled")));

            Resources.AddSpritePack("StandardButton", new Button.SpritePack(
                Content.Load<Texture2D>("UI/button_standard/standard_normal"),
                Content.Load<Texture2D>("UI/button_standard/standard_highlighted"),
                Content.Load<Texture2D>("UI/button_standard/standard_pressed"),
                Content.Load<Texture2D>("UI/button_standard/standard_disabled")));

            Texture2D textboxNormal = Content.Load<Texture2D>("UI/textbox/textbox_normal");
            Texture2D textboxHighlighted = Content.Load<Texture2D>("UI/textbox/textbox_highlighted");
            Texture2D textboxDisabled = Content.Load<Texture2D>("UI/textbox/textbox_disabled");

            Resources.AddSpritePack("Textbox", new Button.SpritePack(
                textboxNormal, textboxHighlighted, textboxHighlighted, textboxDisabled));

            Resources.AddTexture("CartPreview_0", Content.Load<Texture2D>("UI/preview/cart_preview_wooden"));
            Resources.AddTexture("CartPreview_1", Content.Load<Texture2D>("UI/preview/cart_preview_iron"));
            Resources.AddTexture("CartPreview_2", Content.Load<Texture2D>("UI/preview/cart_preview_aluminum"));
            Resources.AddTexture("CartPreview_3", Content.Load<Texture2D>("UI/preview/cart_preview_tank"));
            Resources.AddTexture("CartPreview_4", Content.Load<Texture2D>("UI/preview/cart_preview_golden"));

            Resources.AddTexture("FramedPanel", Content.Load<Texture2D>("UI/panel/panel_framed"));
            Resources.AddTexture("LockIcon", Content.Load<Texture2D>("UI/lock_icon"));
            Resources.AddTexture("MainBackground", Content.Load<Texture2D>("UI/background_images/scene_render_01"));
            Resources.AddTexture("PauseBackground", Content.Load<Texture2D>("UI/background_images/scene_render_02"));
            Resources.AddTexture("GameOverBackground", Content.Load<Texture2D>("UI/background_images/scene_render_03"));

            // LOAD FONTS

            Resources.AddFont("GameTitle", Content.Load<SpriteFont>("Fonts/MainGame_Title_Font"));
            Resources.AddFont("MenuTitle", Content.Load<SpriteFont>("Fonts/MenuTitle_Bahnschrift_Font"));
            Resources.AddFont("MenuItem", Content.Load<SpriteFont>("Fonts/MenuItem_Bahnschrift_Font"));
            Resources.AddFont("MenuSmall", Content.Load<SpriteFont>("Fonts/MenuSmall_Bahnschrift_Font"));
            Resources.AddFont("HUD", Resources.GetFont("MenuItem"));
            Resources.AddFont("DebugInfo", Content.Load<SpriteFont>("Fonts/DebugInfo_Font"));
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
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
                        Collectible.DrawDebugHitbox = !Collectible.DrawDebugHitbox;
                        Obstacle.DrawDebugHitbox = !Obstacle.DrawDebugHitbox;
                        HUD.Instance.ToggleFramerateVisible();
                    }

                    // Update time-related information and parameters

                    timer.Update(gameTime);

                    if (timer.Time.TotalSeconds >= lastSpeedUpdate + speedIncreaseInterval)
                    {
                        Speed = MathHelper.Clamp(Speed + 1f, 0f, maxSpeed);
                        lastSpeedUpdate = (float)timer.Time.TotalSeconds;
                        HUD.Instance.UpdateSpeed(Speed);
                    }

                    HUD.Instance.UpdateTimer(timer);
                    HUD.Instance.UpdateFramerate(1 / gameTime.ElapsedGameTime.TotalSeconds);
                    HUD.Instance.Show(Window);

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
                        CullMode = CullMode.None,
                        MultiSampleAntiAlias = true
                    };

                    // Enable anisotropic texture filtering
                    GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
                    GraphicsDevice.VertexSamplerStates[0] = SamplerState.AnisotropicWrap;

                    // Draw the player and the 3D level (according to the player's POV)

                    player.Draw();
                    level.Draw(player.Camera);

                    // Draw the HUD on top of the rendered scene
                    HUD.Instance.Draw(spriteBatch);

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
                Speed = gameSettings.StartSpeed;
                Score.Current = 0;
                Score.Multiplier = gameSettings.ScoreMultiplier; 
                lastSpeedUpdate = 0f;
                timer.Reset();
                level.Reset();

                // Initialize the Player object
                player = new Player(Resources.GetModel($"Cart_{gameSettings.Cart}"), 
                    new Vector3(0f, 2.5f, -15f), new Vector3(0.8f, 1f, 1.1f), Quaternion.Identity,
                    new Vector3(0f, -2.3f, -0.65f), 1.2f, new Vector3(0f, -0.5f, 0.1f));

                level.InitializeSeed(gameSettings.Seed);
                level.Difficulty = gameSettings.Difficulty;

                // Reset the game HUD
                HUD.Instance.UpdateTimer(timer);
                HUD.Instance.UpdateScore(0);
                HUD.Instance.UpdateSpeed(Speed);

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

                if (Score.Current > Score.Best)
                    Score.Save();
            }
        }

        public void ToMainMenu()
        {
            if (gameState != GameState.MainMenu)
            {
                AudioManager.PlaySong("Cave_MainTheme", true);

                AudioManager.StopAllSoundEffects();      // Immediately stop all sound effects being played

                if (gameState == GameState.Paused)
                {
                    // Save any highscore even if the user leaves the game without losing
                    if (Score.Current > Score.Best)
                        Score.Save();
                }

                mainMenu.Show();
                gameState = GameState.MainMenu;
                IsMouseVisible = true;
            }
        }

    }
}
