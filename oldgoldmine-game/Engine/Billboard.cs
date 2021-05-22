using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Class that represents a textured 2D quad in the 3D scene, always rotated to face towards the camera.
    /// </summary>
    public class Billboard : IPoolable
    {
        // Tool for rendering the textured quad into the scene
        private static readonly BasicEffect renderer = new BasicEffect(OldGoldMineGame.graphics.GraphicsDevice)
        {
            TextureEnabled = true
        };

        // Static quad data, on which the billboard texture is rendered
        private static readonly int[] indices = new int[6] { 0, 1, 2, 2, 1, 3 };
        private static readonly VertexPositionTexture[] vertices = new VertexPositionTexture[4]
        {
            new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0f),  new Vector2(0.0f, 0.0f)),   // Upper left
            new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0f),   new Vector2(1.0f, 0.0f)),   // Upper right
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector2(0.0f, 1.0f)),   // Lower left
            new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0f),  new Vector2(1.0f, 1.0f))    // Lower right
        };


        /// <summary>
        /// The texture applied to this billboard object.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The position of the billboard in 3D space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The scale of the textured quad (billboard).
        /// </summary>
        public Vector2 Scale { get; set; }
        
        /// <summary>
        /// Axis constraint for the billboard rotation (set to zero if unconstrained).
        /// </summary>
        public Vector3 Constraint
        {
            get { return constraint; }
            set 
            { 
                if (value.LengthSquared() > 0)
                    value.Normalize(); 
                constraint = value; 
            }
        }
        private Vector3 constraint;


        /// <summary>
        /// Construct an empty unconstrained Billboard, with default position and scale.
        /// </summary>
        public Billboard()
            : this(null, Vector3.Zero, Vector2.One, Vector3.Zero)
        {
        }

        /// <summary>
        /// Billboard copy constructor.
        /// </summary>
        /// <param name="other">The Billboard object to copy from.</param>
        public Billboard(Billboard other)
            : this(other.Texture, other.Position, other.Scale, other.Constraint)
        {
        }

        /// <summary>
        /// Construct an unconstrained (spherical) Billboard, with the specified texture, position and scale.
        /// </summary>
        /// <param name="texture">The texture drawn by this Billboard.</param>
        /// <param name="position">The initial position of the object.</param>
        /// <param name="scale">The scale factor applied to the quad size.</param>
        public Billboard(Texture2D texture, Vector3 position, Vector2 scale)
            : this(texture, position, scale, Vector3.Zero)
        {
        }

        /// <summary>
        /// Construct a constrained (cylindrical) Billboard, with the specified texture, position, scale and rotation axis.
        /// </summary>
        /// <param name="texture">The texture drawn by this Billboard.</param>
        /// <param name="position">The initial position of the object.</param>
        /// <param name="scale">The scale factor applied to the quad size.</param>
        /// <param name="constraint">The axis around which the Billboard is able to rotate.</param>
        public Billboard(Texture2D texture, Vector3 position, Vector2 scale, Vector3 constraint)
        {
            this.Texture = texture;
            this.Position = position;
            this.Scale = scale;
            this.Constraint = constraint;
        }


        /// <summary>
        /// Render the object in 3D space, according to its position and scale, rotating it towards the camera.
        /// </summary>
        /// <param name="camera">The camera that will be used to render and orientate the Billboard.</param>
        public virtual void Draw(in GameCamera camera)
        {
            if (IsActive && Texture != null)
            {
                renderer.World = Matrix.CreateScale(Scale.X, Scale.Y, 1f);
                if (constraint.LengthSquared() == 0)
                    renderer.World *= Matrix.CreateBillboard(Position, camera.Position, camera.Up, camera.Forward);
                else renderer.World *= Matrix.CreateConstrainedBillboard(Position, camera.Position, constraint, camera.Forward, null);
                renderer.View = OldGoldMineGame.player.Camera.View;
                renderer.Projection = OldGoldMineGame.player.Camera.Projection;
                renderer.Texture = Texture;
                renderer.CurrentTechnique.Passes[0].Apply();

                OldGoldMineGame.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                OldGoldMineGame.graphics.GraphicsDevice.
                    DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
            }
        }


        /// <summary>
        /// Implementation of the Clone() method for the IPoolable interface.
        /// </summary>
        /// <returns>A new Billboard that is an exact copy of this one.</returns>
        public override object Clone()
        {
            return new Billboard(this.Texture, this.Position, this.Scale, this.Constraint);
        }
    }
}
