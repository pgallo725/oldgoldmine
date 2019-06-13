using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using oldgoldmine_game.Menus;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;
using oldgoldmine_game.Gameplay;
using Microsoft.Win32;

namespace oldgoldmine_game
{
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


        public struct GameResources
        {
            /* 3D models for the game */

            public Model m3d_woodenCrate;
            public Model m3d_gold;
            public Model m3d_cart;
            public Model m3d_rails;


            /* Textures and fonts for MenuButton items */

            public Texture2D menuButtonTextureNormal;
            public Texture2D menuButtonTextureHighlighted;
            public Texture2D menuButtonTexturePressed;

            public SpriteFont menuButtonFont;


            /* Textures and fonts for ToggleSelector items */

            public Texture2D leftArrowButtonTextureNormal;
            public Texture2D leftArrowButtonTextureHighlighted;
            public Texture2D leftArrowButtonTexturePressed;

            public Texture2D rightArrowButtonTextureNormal;
            public Texture2D rightArrowButtonTextureHighlighted;
            public Texture2D rightArrowButtonTexturePressed;

            public Texture2D plusButtonTextureNormal;
            public Texture2D plusButtonTextureHighlighted;
            public Texture2D plusButtonTexturePressed;

            public Texture2D minusButtonTextureNormal;
            public Texture2D minusButtonTextureHighlighted;
            public Texture2D minusButtonTexturePressed;

            public SpriteFont settingSelectorFont;


            /* Other fonts */

            public SpriteFont menuTitleFont;
            public SpriteFont hudFont;
            public SpriteFont debugInfoFont;
        }

        public static Player player;
        public static Timer timer;

        private static OldGoldMineGame application;
        public static OldGoldMineGame Application { get { return application; } }

        public static GameResources resources = new GameResources();

        GameState gameState;

        HUD hud = new HUD();
        MainMenu mainMenu = new MainMenu();
        CustomizationMenu customMenu = new CustomizationMenu();
        PauseMenu pauseMenu = new PauseMenu();
        GameOverMenu deathMenu = new GameOverMenu();
        

        Queue<GameObject3D> rails;
        Queue<Collectible> collectibles;
        Queue<Obstacle> obstacles;
        ProceduralGenerator levelGenerator;
        

        private static int score = 0;
        public static int Score { get { return score; } set { score = value; } }
        private static int bestScore = 0;
        public static int BestScore { get { return bestScore; } set { bestScore = value; } }

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
            //this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 4);
            OldGoldMineGame.graphics.SynchronizeWithVerticalRetrace = false;
            OldGoldMineGame.application = this;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Window.Title = "The Old Gold Mine (Pre-Alpha)";
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


            // Initialize Camera and Player objects
            player = new Player();
            GameCamera camera = new GameCamera();
            camera.Initialize(new Vector3(0f, 2.5f, -15f), Vector3.Zero, GraphicsDevice.DisplayMode.AspectRatio);
            player.Initialize(camera, 
                new GameObject3D(resources.m3d_cart, Vector3.Zero, new Vector3(0.8f, 1f, 1.1f), Quaternion.Identity),
                new Vector3(0f, -2.4f, -0.75f), 1.2f);

            BestScore = LoadScore();

            // Initialize menus
            mainMenu.Initialize(GraphicsDevice, null);
            customMenu.Initialize(GraphicsDevice, null);
            pauseMenu.Initialize(GraphicsDevice, null);
            deathMenu.Initialize(GraphicsDevice, null);
            

            // Setup HUD
            timer = new Timer();
            hud.Initialize(Window);
            

            // Instantiate and initialize the pool of items for the procedural generator
            rails = new Queue<GameObject3D>();
            collectibles = new Queue<Collectible>();
            obstacles = new Queue<Obstacle>();

            levelGenerator = new ProceduralGenerator(in rails, resources.m3d_rails, 20f,
                in collectibles, resources.m3d_gold, 0.25f,
                in obstacles, resources.m3d_woodenCrate, new Vector3(2f, 2f, 2f),
                150f);


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
            resources.menuButtonTextureNormal = Content.Load<Texture2D>("ui_elements_2d/woodButton_normal");
            resources.menuButtonTextureHighlighted = Content.Load<Texture2D>("ui_elements_2d/woodButton_highlighted");
            resources.menuButtonFont = Content.Load<SpriteFont>("ui_elements_2d/MenuFont");
            resources.debugInfoFont = Content.Load<SpriteFont>("ui_elements_2d/SmallFont");

            resources.hudFont = resources.menuButtonFont;   // tmp
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

                    timer.Update(gameTime);

                    if (timer.time.TotalSeconds >= lastSpeedUpdate + speedIncreaseInterval)
                    {
                        Speed = MathHelper.Clamp(Speed + 1f, 0, maxSpeed);
                        lastSpeedUpdate = (float)gameTime.TotalGameTime.TotalSeconds;
                        hud.UpdateSpeed(Speed);
                    }

                    hud.UpdateTimer(timer);
                    hud.UpdateFramerate(1 / gameTime.ElapsedGameTime.TotalSeconds);
                    hud.UpdateScore(Score);     // TODO: do it only when really needed (on collectible pickup)

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

                    player.LookUpDown(InputManager.MouseMovementY * lookAroundSpeed, freeMovement);
                    player.LookLeftRight(InputManager.MouseMovementX * lookAroundSpeed, freeMovement);

                    player.Update(gameTime);
                    levelGenerator.Update(player.Position, rails, collectibles, obstacles);

                    foreach (Collectible gold in collectibles)
                    {
                        gold.Update();
                    }

                    foreach (Obstacle obstacle in obstacles)
                    {
                        obstacle.Update();
                    }

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

                    // Draw 3D objects in the scene, starting from the player

                    player.Draw();

                    foreach(GameObject3D rail in rails)
                    {
                        rail.Draw(player.Camera);
                    }

                    foreach (Collectible gold in collectibles)
                    {
                        gold.Draw(player.Camera);
                    }

                    foreach (Obstacle obstacle in obstacles)
                    {
                        obstacle.Draw(player.Camera);
                    }

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
                gameState = GameState.NewGame;
                IsMouseVisible = true;
                // anything else ?
            }
        }

        public void StartGame()
        {
            if (gameState == GameState.NewGame)
            {
                // TODO: reload the entire scene to its initial state
                Score = 0;
                gameState = GameState.Running;
                IsMouseVisible = false;
                // anything else ?
            }
        }

        public void PauseGame()
        {
            if (gameState == GameState.Running)
            {
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
                if (Score > BestScore)
                {
                    BestScore = Score;
                    SaveScore(Score);
                } 
                gameState = GameState.GameOver;
                IsMouseVisible = true;
                // TODO: destroy the player object
            }
        }

        public void ToMainMenu()
        {
            if (gameState != GameState.MainMenu)
            {
                gameState = GameState.MainMenu;
                IsMouseVisible = true;
                // anything else ?
            }
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
            try {
                score = (int)Registry.GetValue(key, "Best_Score", 0); //without ? it's not nullable
                if (score == null)
                    score = 0;
            }
            catch (System.Exception) {
                //nothing
            }
            return score.Value;
        }
    }
}
