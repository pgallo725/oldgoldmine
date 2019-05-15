using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using oldgoldmine_game.Menus;
using oldgoldmine_game.Engine;

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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameCamera camera = new GameCamera();
        GameState gameState;

        MainMenu mainMenu = new MainMenu();
        PauseMenu pauseMenu = new PauseMenu();
        GameOverMenu deathMenu = new GameOverMenu();


        GameObject3D woodenCrate;
        GameObject3D pickaxe;
        GameObject3D lantern;
        GameObject3D sack;

        Model m3d_woodenCrate;
        Model m3d_pickaxe;
        Model m3d_lantern;
        Model m3d_sack;

        Texture2D buttonTextureNormal;
        Texture2D buttonTextureHighlighted;
        SpriteFont menuFont;

        
        public OldGoldMineGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 2);
            this.graphics.SynchronizeWithVerticalRetrace = false;

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

            // Initialize Camera object
            camera.Initialize(new Vector3(0f, 0f, -15f), Vector3.Zero, GraphicsDevice.DisplayMode.AspectRatio);

            // Initialize menus
            mainMenu.Initialize(GraphicsDevice, null, menuFont, buttonTextureNormal, buttonTextureHighlighted);
            pauseMenu.Initialize(GraphicsDevice, null, menuFont, buttonTextureNormal, buttonTextureHighlighted);
            deathMenu.Initialize(GraphicsDevice, null, menuFont, buttonTextureNormal, buttonTextureHighlighted);

            // Create GameObjects for the imported 3D models and set their position, rotation and scale
            woodenCrate = new GameObject3D(m3d_woodenCrate);
            pickaxe = new GameObject3D(m3d_pickaxe);
            lantern = new GameObject3D(m3d_lantern);
            sack = new GameObject3D(m3d_sack);

            woodenCrate.EnableLightingModel();
            pickaxe.EnableLightingModel();
            lantern.EnableLightingModel();
            sack.EnableLightingModel();

            pickaxe.RotateAroundAxis(Vector3.Up, 90f);
            pickaxe.RotateAroundAxis(Vector3.Right, 170f);
            lantern.RotateAroundAxis(Vector3.Up, 10f);
            sack.RotateAroundAxis(Vector3.Up, 110f);
            sack.RotateAroundAxis(Vector3.Backward, 5f);

            pickaxe.Position = new Vector3(3f, 0.5f, 0f);
            lantern.Position = new Vector3(-0.6f, 4.5f, -0.6f);
            sack.Position = new Vector3(-2.5f, -2.75f, -3.25f);

            woodenCrate.ScaleSize(2.5f);
            pickaxe.ScaleSize(1.1f);
            lantern.ScaleSize(0.6f);
            sack.ScaleSize(0.6f);

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

            m3d_woodenCrate = Content.Load<Model>("models_3d/woodenCrate");
            m3d_pickaxe = Content.Load<Model>("models_3d/pickaxe_lowpoly");
            m3d_lantern = Content.Load<Model>("models_3d/lantern_lowpoly");
            m3d_sack = Content.Load<Model>("models_3d/sack_lowpoly");

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

                    float moveSpeed = 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    float rotationSpeed = 60f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        camera.Move(moveSpeed, camera.Forward);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        camera.Move(moveSpeed, camera.Back);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        camera.Move(moveSpeed, camera.Left);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        camera.Move(moveSpeed, camera.Right);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        camera.Move(moveSpeed, camera.Up);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                    {
                        camera.Move(moveSpeed, camera.Down);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        camera.RotateViewVertical(rotationSpeed);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        camera.RotateViewVertical(-rotationSpeed);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        camera.RotateViewHorizontal(-rotationSpeed);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        camera.RotateViewHorizontal(rotationSpeed);
                    }

                    camera.Update();

                    break;
                }

                case GameState.Paused:
                {
                    pauseMenu.Update(this);
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

                    spriteBatch.Begin();

                    double fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;
                    spriteBatch.DrawString(menuFont,                            // Print framerate information
                        fps.ToString(" 0.# FPS"),
                        new Vector2(5, 5), 
                        fps < 60f ? Color.Red : Color.Green);

                    spriteBatch.End();


                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    woodenCrate.Draw(camera);
                    pickaxe.Draw(camera);
                    lantern.Draw(camera);
                    sack.Draw(camera);

                    
                    break;
                }

                case GameState.Paused:
                {
                    pauseMenu.Draw(GraphicsDevice, spriteBatch);
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
