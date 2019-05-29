using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using oldgoldmine_game.Menus;
using oldgoldmine_game.Engine;
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

        private static OldGoldMineGame application;
        public static OldGoldMineGame Application { get { return application; } }

        GameState gameState;

        MainMenu mainMenu = new MainMenu();
        PauseMenu pauseMenu = new PauseMenu();
        GameOverMenu deathMenu = new GameOverMenu();


        GameObject3D cart;
        Collectible gold;
        Obstacle box;

        List<GameObject3D> rails;
        //ObjectPool<GameObject3D> railPool;

        Model m3d_woodenCrate;
        Model m3d_pickaxe;
        Model m3d_lantern;
        Model m3d_sack;
        Model m3d_gold;
        Model m3d_cart;
        Model m3d_rails;

        Texture2D buttonTextureNormal;
        Texture2D buttonTextureHighlighted;
        SpriteFont menuFont;

        private static int score = 0;
        public static int Score { get { return score; } set { score = value; } }

        bool freeMovement = false;
        float nextRailsGenerationPosition = 40f;
        float popupDistance = 40f;
        float currentSpeed = 10f;
        
        public OldGoldMineGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 4);
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

            // Initialize player and Player objects
            player = new Player();
            GameCamera camera = new GameCamera();
            camera.Initialize(new Vector3(0f, 1.5f, -15f), Vector3.Zero, GraphicsDevice.DisplayMode.AspectRatio);
            player.Initialize(camera);

            // Initialize menus
            mainMenu.Initialize(GraphicsDevice, null, menuFont, buttonTextureNormal, buttonTextureHighlighted);
            pauseMenu.Initialize(GraphicsDevice, null, menuFont, buttonTextureNormal, buttonTextureHighlighted);
            deathMenu.Initialize(GraphicsDevice, null, menuFont, buttonTextureNormal, buttonTextureHighlighted);

            // Create GameObjects from the imported 3D models and set their position, rotation and scale
            gold = new Collectible(m3d_gold);
            cart = new GameObject3D(m3d_cart);
            box = new Obstacle(new GameObject3D(m3d_woodenCrate), new Vector3(2.2f, 2.2f, 2.2f));
            rails = new List<GameObject3D>();
            rails.Add(new GameObject3D(m3d_rails));
            rails.Add(new GameObject3D(m3d_rails, new Vector3(0f, 0f, 20f), Vector3.One, Quaternion.Identity));

            gold.EnableLightingModel();
            cart.EnableLightingModel();
            box.EnableLightingModel();
            rails[0].EnableLightingModel();
            rails[1].EnableLightingModel();

            gold.Position = new Vector3(-4.5f, 5f, -2f);
            cart.Position = new Vector3(10f, 0.5f, 0f);
            box.Position = new Vector3(10f, 0.5f, -3f);

            gold.ScaleSize(1.5f);
            cart.ScaleSize(2f);


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

            m3d_woodenCrate = Content.Load<Model>("models_3d/woodenCrate");
            m3d_pickaxe = Content.Load<Model>("models_3d/pickaxe_lowpoly");
            m3d_lantern = Content.Load<Model>("models_3d/lantern_lowpoly");
            m3d_sack = Content.Load<Model>("models_3d/sack_lowpoly");
            m3d_gold = Content.Load<Model>("models_3d/goldOre");
            m3d_cart = Content.Load<Model>("models_3d/cart_lowpoly");
            m3d_rails = Content.Load<Model>("models_3d/rails_segment");

            buttonTextureNormal = Content.Load<Texture2D>("ui_elements_2d/woodButton_normal");
            buttonTextureHighlighted = Content.Load<Texture2D>("ui_elements_2d/woodButton_highlighted");
            menuFont = Content.Load<SpriteFont>("ui_elements_2d/MenuFont");
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
                    mainMenu.Update(this);
                    break;
                }
                    
                case GameState.Running:
                {
                    if (InputManager.PauseKeyPressed)
                        PauseGame();

                    if (InputManager.DebugKeyPressed)
                        freeMovement = true;

                    float moveSpeed = currentSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    float rotationSpeed = 60f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    float lookAroundSpeed = 80f * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

                        if (Keyboard.GetState().IsKeyDown(Keys.Up))
                        {
                            player.RotateUpDown(rotationSpeed);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Down))
                        {
                            player.RotateUpDown(-rotationSpeed);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Left))
                        {
                            player.RotateLeftRight(-rotationSpeed);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Right))
                        {
                            player.RotateLeftRight(rotationSpeed);
                        }
                    }
                    else
                    {
                        player.Move(moveSpeed, Vector3.Backward);
                    }

                    player.LookUpDown(InputManager.MouseMovementY * lookAroundSpeed);
                    player.LookLeftRight(InputManager.MouseMovementX * lookAroundSpeed);

                    player.Update();

                    if (player.Position.Z >= nextRailsGenerationPosition - popupDistance)
                    {
                        ProceduralRailsGeneration();
                    }

                    gold.Update();
                    box.Update();

                    break;
                }

                case GameState.Paused:
                {
                    pauseMenu.Update(this);
                    break;
                }

                case GameState.GameOver:
                {
                    deathMenu.Update(this);
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

                    gold.Draw(player.Camera);
                    //cart.Draw(player.Camera);
                    box.Draw(player.Camera);

                    foreach(GameObject3D rail in rails)
                    {
                        rail.Draw(player.Camera);
                    }


                    spriteBatch.Begin();

                    double fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;
                    spriteBatch.DrawString(menuFont,                            // Print framerate information
                        fps.ToString(" 0.# FPS"),
                        new Vector2(5, 5),
                        fps < 60f ? Color.Red : Color.Green);

                    spriteBatch.DrawString(menuFont,                            // Show score
                        score.ToString(" Score: 0.#"),
                        new Vector2(5, 50),
                        Color.White);

                    spriteBatch.End();

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


        private void ProceduralRailsGeneration()
        {
            rails.Add(new GameObject3D(m3d_rails, new Vector3(0f, 0f, nextRailsGenerationPosition),
                Vector3.One, Quaternion.Identity));
            nextRailsGenerationPosition += 20f;
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
