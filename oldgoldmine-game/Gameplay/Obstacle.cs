using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;


namespace OldGoldMine.Gameplay
{
    public class Obstacle : GameObject3D
    {
        public static bool DrawDebugHitbox = false;
        private static Color DebugColor = Color.Red;

        private BoundingBox hitbox;

        public override Vector3 Position
        {
            get { return base.position; }
            set
            {
                Vector3 deltaMovement = value - base.Position;
                base.Position = value;
                hitbox.Min += deltaMovement;
                hitbox.Max += deltaMovement;
            }
        }

        public override Vector3 Scale
        {
            get { return base.scale; }
            set
            {
                Vector3 hitboxOffset = (hitbox.Max + hitbox.Min) / 2 - base.Position;
                hitboxOffset = (hitboxOffset / base.Scale) * value;
                Vector3 boxSize = ((hitbox.Max - hitbox.Min) / base.Scale) * value;
                base.Scale = value;
                hitbox.Min = Position + hitboxOffset - boxSize / 2;
                hitbox.Max = Position + hitboxOffset + boxSize / 2;
            }
        }


        // Create the bounding box by taking the bounds of all the model meshes
        // and merging them together into a single bounding box
        private static BoundingBox CreateBoundingBoxFromModel(Model model, Vector3 position, Vector3 scale)
        {
            BoundingBox hitbox = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);
            for (int meshIndex = 1; meshIndex < model.Meshes.Count; meshIndex++)
            {
                BoundingBox meshBox = BoundingBox.CreateFromSphere(model.Meshes[meshIndex].BoundingSphere);
                hitbox = BoundingBox.CreateMerged(hitbox, meshBox);
            }

            Vector3 hitboxSize = (hitbox.Max - hitbox.Min) * scale;
            hitbox.Min = position - hitboxSize / 2;
            hitbox.Max = position + hitboxSize / 2;

            return hitbox;
        }


        /// <summary>
        /// Default construction of an empty Obstacle object, provided
        /// for the type to be used in an ObjectPool.
        /// </summary>
        public Obstacle()
            : base()
        {
            this.hitbox = new BoundingBox();
        }

        /// <summary>
        /// Construct an Obstacle object with a 3D model and the provided hitbox.
        /// </summary>
        /// <param name="model">The 3D model for this Obstacle object.</param>
        /// <param name="position">Position of the object in world space.</param>
        /// <param name="scale">Scale applied to the model and hitbox.</param>
        /// <param name="rotation">Rotation of the object in world space.</param>
        /// <param name="hitbox">The hitbox that will be attached to this object.</param>
        public Obstacle(Model model, Vector3 position, Vector3 scale, Quaternion rotation, BoundingBox hitbox)
            : base(model, position, scale, rotation)
        {
            this.hitbox = hitbox;
        }

        /// <summary>
        /// Construct an Obstacle object with a 3D model and create its hitbox "in place" using the specified size.
        /// </summary>
        /// <param name="model">The 3D model for this Obstacle object.</param>
        /// <param name="position">Position of the object in world space.</param>
        /// <param name="scale">Scale applied to the model and hitbox.</param>
        /// <param name="rotation">Rotation of the object in world space.</param>
        /// <param name="hitboxSize">The size (on each axis) of the object's hitbox.</param>
        public Obstacle(Model model, Vector3 position, Vector3 scale, Quaternion rotation, Vector3 hitboxSize)
            : this(model, position, scale, rotation, 
                  new BoundingBox(position - hitboxSize / 2, position + hitboxSize / 2))
        {
        }

        /// <summary>
        /// Construct an Obstacle object with a 3D model and an automatically generated hitbox.
        /// </summary>
        /// <param name="model">The 3D model for this Obstacle object.</param>
        /// <param name="position">Position of the object in world space.</param>
        /// <param name="scale">Scale applied to the model and hitbox.</param>
        /// <param name="rotation">Rotation of the object in world space.</param>
        public Obstacle(Model model, Vector3 position, Vector3 scale, Quaternion rotation)
            : this(model, position, scale, rotation, CreateBoundingBoxFromModel(model, position, scale))
        {
        }

        
        /// <summary>
        /// Update the object's status in the current frame.
        /// </summary>
        /// <param name="gameTime">Time signature of the current frame.</param>
        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            // Check if the player has hit the obstacle
            if (this.hitbox.Intersects(OldGoldMineGame.player.Hitbox))
            {
                this.IsActive = false;
                AudioManager.PlaySoundEffect("Crash_Sound");
                OldGoldMineGame.Application.GameOver();
            }
        }


        public override void Draw(in GameCamera camera)
        {
            if (!IsActive)
                return;

            base.Draw(camera);

            if (DrawDebugHitbox)
            {
                OldGoldMineGame.basicEffect.Projection = OldGoldMineGame.player.Camera.Projection;
                OldGoldMineGame.basicEffect.View = OldGoldMineGame.player.Camera.View;

                Vector3[] vertices = hitbox.GetCorners();

                // Pairs of points define the lines (segments) which are the border of the box to draw
                VertexPositionColor[] lineVertices = new VertexPositionColor[24]
                {
                    new VertexPositionColor(vertices[0], DebugColor),
                    new VertexPositionColor(vertices[1], DebugColor),
                    new VertexPositionColor(vertices[0], DebugColor),
                    new VertexPositionColor(vertices[4], DebugColor),
                    new VertexPositionColor(vertices[0], DebugColor),
                    new VertexPositionColor(vertices[3], DebugColor),
                    new VertexPositionColor(vertices[1], DebugColor),
                    new VertexPositionColor(vertices[2], DebugColor),
                    new VertexPositionColor(vertices[1], DebugColor),
                    new VertexPositionColor(vertices[5], DebugColor),
                    new VertexPositionColor(vertices[2], DebugColor),
                    new VertexPositionColor(vertices[3], DebugColor),
                    new VertexPositionColor(vertices[2], DebugColor),
                    new VertexPositionColor(vertices[6], DebugColor),
                    new VertexPositionColor(vertices[3], DebugColor),
                    new VertexPositionColor(vertices[7], DebugColor),
                    new VertexPositionColor(vertices[7], DebugColor),
                    new VertexPositionColor(vertices[4], DebugColor),
                    new VertexPositionColor(vertices[4], DebugColor),
                    new VertexPositionColor(vertices[5], DebugColor),
                    new VertexPositionColor(vertices[5], DebugColor),
                    new VertexPositionColor(vertices[6], DebugColor),
                    new VertexPositionColor(vertices[6], DebugColor),
                    new VertexPositionColor(vertices[7], DebugColor)
                };

                OldGoldMineGame.basicEffect.CurrentTechnique.Passes[0].Apply();
                OldGoldMineGame.graphics.GraphicsDevice.
                    DrawUserPrimitives(PrimitiveType.LineList, lineVertices, 0, 12);
            }
        }


        public override object Clone()
        {
            return new Obstacle(this.model3d, this.position, this.scale, this.rotation, this.hitbox);
        }

    }
}
