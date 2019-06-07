using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using oldgoldmine_game.Menus;
using oldgoldmine_game.Engine;
using oldgoldmine_game.UI;
using oldgoldmine_game.Gameplay;

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
            Running,
            Paused,
            GameOver
        }

        // Statically accessible graphics and rendering tools
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static BasicEffect basicEffect;

        public static Player player;
        public static Timer timer;

        private static OldGoldMineGame application;
        public static OldGoldMineGame Application { get { return application; } }

        GameState gameState;

        HUD hud = new HUD();
        MainMenu mainMenu = new MainMenu();
        PauseMenu pauseMenu = new PauseMenu();
        GameOverMenu deathMenu = new GameOverMenu();
        

        Queue<GameObject3D> rails;
        Queue<Collectible> collectibles;
        ProceduralGenerator levelGenerator;

        Collectible gold;
        Obstacle box;


        Model m3d_woodenCrate;
        Model m3d_gold;
        Model m3d_cart;
        Model m3d_rails;

        Texture2D buttonTextureNormal;
        Texture2D buttonTextureHighlighted;
        SpriteFont largeFont;
        SpriteFont smallFont;

        private static int score = 0;
        public static int Score { get { return score; } set { score = value; } }

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

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 576;

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
                new GameObject3D(m3d_cart, Vector3.Zero, new Vector3(0.8f, 1f, 1.1f), Quaternion.Identity),
                new Vector3(0f, -2.4f, -0.75f));

            // Initialize menus
            mainMenu.Initialize(GraphicsDevice, null, largeFont, buttonTextureNormal, buttonTextureHighlighted);
            pauseMenu.Initialize(GraphicsDevice, null, largeFont, buttonTextureNormal, buttonTextureHighlighted);
            deathMenu.Initialize(GraphicsDevice, null, largeFont, buttonTextureNormal, buttonTextureHighlighted);

            // Setup HUD
            timer = new Timer();
            hud.Initialize(Window, smallFont, largeFont);
            

            // Create GameObjects from the imported 3D models and set their position, rotation and scale
            gold = new Collectible(m3d_gold);
            box = new Obstacle(new GameObject3D(m3d_woodenCrate), new Vector3(2.2f, 2.2f, 2.2f));

            // Instantiate and initialize the pool of rail segments to be drawn
            rails = new Queue<GameObject3D>();
            collectibles = new Queue<Collectible>();

            levelGenerator = new ProceduralGenerator(in rails, m3d_rails, 20f, in collectibles, m3d_gold, 150f);


            // Prepare 3D game objects for the scene
            gold.EnableLightingModel();
            box.EnableLightingModel();
            gold.Position = new Vector3(0f, 1.25f, 20f);
            box.Position = new Vector3(10f, 0.5f, -3f);
            gold.ScaleSize(0.25f);


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
            m3d_woodenCrate = Content.Load<Model>("models_3d/woodenCrate");
            m3d_gold = Content.Load<Model>("models_3d/goldOre");
            m3d_cart = Content.Load<Model>("models_3d/cart_lowpoly");
            m3d_rails = Content.Load<Model>("models_3d/rails_segment");

            // Load 2D elements for UI graphics
            buttonTextureNormal = Content.Load<Texture2D>("ui_elements_2d/woodButton_normal");
            buttonTextureHighlighted = Content.Load<Texture2D>("ui_elements_2d/woodButton_highlighted");
            largeFont = Content.Load<SpriteFont>("ui_elements_2d/MenuFont");
            smallFont = Content.Load<SpriteFont>("ui_elements_2d/SmallFont");
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
                    float lookAroundSpeed = 60f * (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                    levelGenerator.Update(player.Position, rails, collectibles);

                    gold.Update();
                    box.Update();

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
                    gold.Draw(player.Camera);
                    box.Draw(player.Camera);

                    foreach(GameObject3D rail in rails)
                    {
                        rail.Draw(player.Camera);
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


        public void StartGame()
        {
            if (gameState == GameState.MainMenu)
            {
                // TODO: reload the entire scene to its initial state
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

    }
}
