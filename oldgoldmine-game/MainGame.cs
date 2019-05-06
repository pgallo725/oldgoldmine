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

        Vector3 camTarget;
        Vector3 camPosition;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        /*
        BasicEffect basicEffect;
        VertexPositionColor[] triangleVertices;
        VertexBuffer vertexBuffer;
        */

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

            // Setup Camera
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(0f, 0f, -12f);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60f),                              // 60º FOV
                GraphicsDevice.DisplayMode.AspectRatio,                 // Same aspect ratio as the screen
                1f, 100f);                                              // Clipping planes (near and far)

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

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

            /*
            // BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            // Create triangle
            triangleVertices = new VertexPositionColor[3];
            triangleVertices[0] = new VertexPositionColor(new Vector3(0, 2, 0), Color.Red);
            triangleVertices[1] = new VertexPositionColor(new Vector3(-2, -2, 0), Color.Green);
            triangleVertices[2] = new VertexPositionColor(new Vector3(2, -2, 0), Color.Blue);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(triangleVertices);
            */
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
                Vector3 camDirection = camPosition - camTarget;

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    Vector3 camMovement = new Vector3(-camDirection.Z, 0f, camDirection.X);
                    camPosition += (4f * (float)gameTime.ElapsedGameTime.TotalSeconds) * camMovement;
                    camTarget += (4f * (float)gameTime.ElapsedGameTime.TotalSeconds) * camMovement;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    Vector3 camMovement = new Vector3(-camDirection.Z, 0f, camDirection.X);
                    camPosition -= (4f * (float)gameTime.ElapsedGameTime.TotalSeconds) * camMovement;
                    camTarget -= (4f * (float)gameTime.ElapsedGameTime.TotalSeconds) * camMovement;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    camPosition.Y -= (4f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    camTarget.Y -= (4f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    camPosition.Y += (4f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    camTarget.Y += (4f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                {
                    camPosition -= (4f * (float)gameTime.ElapsedGameTime.TotalSeconds) * camDirection;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                {
                    camPosition += (4f * (float)gameTime.ElapsedGameTime.TotalSeconds) * camDirection;
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
                    Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(60f) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    camPosition = Vector3.Transform(camPosition, rotationMatrix);
                }

                viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

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

            /*
            basicEffect.Projection = projectionMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
            }
            */

            woodenCrate.Draw(cratePosition * (crateRotation * (crateScale * worldMatrix)), viewMatrix, projectionMatrix);
            pickaxe.Draw(pickaxePosition * (pickaxeRotation * (pickaxeScale * worldMatrix)), viewMatrix, projectionMatrix);
            lantern.Draw(lanternPosition * (lanternRotation * (lanternScale * worldMatrix)), viewMatrix, projectionMatrix);
            sack.Draw(sackPosition * (sackRotation * (sackScale * worldMatrix)), viewMatrix, projectionMatrix);


            double fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;     // Print framerate informations
            Window.Title = fps.ToString("The Old Gold Mine (Pre-Alpha) - Framerate: 0.# FPS");


            base.Draw(gameTime);
        }
    }
}
