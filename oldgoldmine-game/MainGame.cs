using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace oldgoldmine_game
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OldGoldMineGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameCamera camera;

        Matrix worldMatrix;


        Model woodenCrate;
        Matrix cratePosition;
        Matrix crateRotation;
        Matrix crateScale;
        Model pickaxe;
        Matrix pickaxePosition;
        Matrix pickaxeRotation;
        Matrix pickaxeScale;
        Model lantern;
        Matrix lanternPosition;
        Matrix lanternRotation;
        Matrix lanternScale;
        Model sack;
        Matrix sackPosition;
        Matrix sackRotation;
        Matrix sackScale;


        bool orbit;
        bool spaceWasPressed;

        public OldGoldMineGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 2);
            this.graphics.SynchronizeWithVerticalRetrace = false;
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

            // Create and initialize Camera object
            camera = new GameCamera();
            camera.Initialize(new Vector3(0f, 0f, -15f), Vector3.Zero, GraphicsDevice.DisplayMode.AspectRatio);

            worldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            cratePosition = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            crateRotation = Matrix.CreateRotationY(MathHelper.ToRadians(0f));
            crateScale = Matrix.CreateScale(2.5f);

            pickaxePosition = Matrix.CreateTranslation(new Vector3(0, 0.5f, 3f));
            pickaxeRotation = Matrix.CreateRotationY(MathHelper.ToRadians(90f))
                * (Matrix.CreateRotationX(MathHelper.ToRadians(180f))
                * Matrix.CreateRotationZ(MathHelper.ToRadians(15f)));
            pickaxeScale = Matrix.CreateScale(1.1f);

            lanternPosition = Matrix.CreateTranslation(new Vector3(-1, 7.5f, -1));
            lanternRotation = Matrix.CreateRotationY(MathHelper.ToRadians(10f));
            lanternScale = Matrix.CreateScale(0.6f);

            sackPosition = Matrix.CreateTranslation(new Vector3(6.25f, -5f, -1));
            sackRotation = Matrix.CreateRotationY(MathHelper.ToRadians(120f))
                * Matrix.CreateRotationX(MathHelper.ToRadians(10f));
            sackScale = Matrix.CreateScale(0.6f);


            PrepareModel(woodenCrate/*, cratePosition * (crateRotation * (crateScale * worldMatrix))*/);
            PrepareModel(pickaxe/*, pickaxePosition * (pickaxeRotation * (pickaxeScale * worldMatrix))*/);
            PrepareModel(lantern/*, lanternPosition * (lanternRotation * (lanternScale * worldMatrix))*/);
            PrepareModel(sack/*, sackPosition * (sackRotation * (sackScale * worldMatrix))*/);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            woodenCrate = Content.Load<Model>("models_3d/woodenCrate");
            pickaxe = Content.Load<Model>("models_3d/pickaxe_lowpoly");
            lantern = Content.Load<Model>("models_3d/lantern_lowpoly");
            sack = Content.Load<Model>("models_3d/sack_lowpoly");
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
            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                float moveSpeed = 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    camera.Move(moveSpeed, camera.Forward);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    camera.Move(moveSpeed, camera.Back);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    camera.Move(moveSpeed, camera.Left);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    camera.Move(moveSpeed, camera.Right);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                {
                    if (!orbit) camera.Move(moveSpeed, camera.Up);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                {
                    if (!orbit) camera.Move(moveSpeed, camera.Down);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    if (!spaceWasPressed)
                        orbit = !orbit;

                    spaceWasPressed = true;
                }
                else spaceWasPressed = false;

                if (orbit)
                {
                    camera.LookAt(Vector3.Zero);
                    float rotation = 60f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    camera.RotateAroundTargetY(rotation);
                }

                camera.Update();
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            woodenCrate.Draw(cratePosition * (crateRotation * (crateScale * worldMatrix)), camera.View, camera.Projection);
            pickaxe.Draw(pickaxePosition * (pickaxeRotation * (pickaxeScale * worldMatrix)), camera.View, camera.Projection);
            lantern.Draw(lanternPosition * (lanternRotation * (lanternScale * worldMatrix)), camera.View, camera.Projection);
            sack.Draw(sackPosition * (sackRotation * (sackScale * worldMatrix)), camera.View, camera.Projection);

            


            double fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;     // Print framerate informations
            Window.Title = fps.ToString("The Old Gold Mine (Pre-Alpha) - Framerate: 0.# FPS");


            base.Draw(gameTime);
        }


        /// <summary>
        /// Provides a quick way to draw a model in the scene after setting 
        /// all its BasicEffects properties to the desired values
        /// </summary>
        void PrepareModel(Model model/*, Matrix positionMatrix*/)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    //effect.World = positionMatrix;
                    //effect.View = camera.View;
                    //effect.Projection = camera.Projection;
                }

                // Draw the entire mesh
                //mesh.Draw();
            }
        }

    }
}
