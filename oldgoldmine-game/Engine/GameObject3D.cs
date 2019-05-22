using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace oldgoldmine_game.Engine
{

    public class GameObject3D
    {
        private readonly Model model3d;

        private Vector3 position;
        private Vector3 scale;
        private Quaternion rotation;

        protected bool active = true;


        public virtual Vector3 Position { get { return position; } set { position = value; } }
        public virtual Vector3 Scale { get { return scale; } set { scale = value; } }
        public virtual Quaternion Rotation { get { return rotation; } set { rotation = value; } }

        // When a GameObject is active, it's rendered on screen and 
        public bool IsActive { get { return active; } set { active = value; } }

        private Matrix objectWorldMatrix;
        private bool updated = false;

        /// <summary>
        /// Get the world matrix representing the coordinate system of this object, incorporating any position,
        /// rotation and scale transformation that has been applied to it. Use this for rendering the 3D model.
        /// </summary>
        public Matrix ObjectWorldMatrix { get { if (!updated) this.Update(); return objectWorldMatrix; } }


        /// <summary>
        /// Construct an empty GameObject3D, with default position, rotation and size.
        /// </summary>
        public GameObject3D()
        {
            this.model3d = null;
            this.position = Vector3.Zero;
            this.scale = Vector3.One;
            this.rotation = Quaternion.Identity;
        }

        /// <summary>
        /// GameObject3D copy constructor.
        /// </summary>
        public GameObject3D(GameObject3D other)
        {
            this.model3d = other.model3d;
            this.position = other.position;
            this.scale = other.scale;
            this.rotation = other.rotation;
            this.active = other.active;
        }

        /// <summary>
        /// Construct a GameObject3D around an imported Model, with default position, rotation and size.
        /// </summary>
        /// <param name="model">The 3D model of this object.</param>
        public GameObject3D(Model model)
        {
            this.model3d = model;
            this.position = Vector3.Zero;
            this.scale = Vector3.One;
            this.rotation = Quaternion.Identity;
        }

        /// <summary>
        /// Construct a GameObject3D around an imported Model, initializing its position, rotation and size.
        /// </summary>
        /// <param name="model">The 3D model of this object.</param>
        /// <param name="position">The initial position of the object.</param>
        /// <param name="scale">The scale factor applied to the model's size.</param>
        /// <param name="rotation">The initial rotation of the 3D model.</param>
        public GameObject3D(Model model, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            this.model3d = model;
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }


        /// <summary>
        /// Change the position of this object in 3D coordinate space.
        /// </summary>
        /// <param name="movement">A Vector3 representing the amount of movement to apply on each axis.</param>
        public void MovePosition(Vector3 movement)
        {
            position += movement;
            updated = false;
        }

        /// <summary>
        /// Change the scale (size) of the 3D model.
        /// </summary>
        /// <param name="scale">A value representing the uniform scaling factor for the entire object.</param>
        public void ScaleSize(float scale)
        {
            this.scale.X = scale;
            this.scale.Y = scale;
            this.scale.Z = scale;
            updated = false;
        }

        /// <summary>
        /// Change the scale (size) of the 3D model.
        /// </summary>
        /// <param name="scale">A Vector3 representing the scaling factors for each axis.</param>
        public void ScaleSize(Vector3 scale)
        {
            this.scale = scale;
            updated = false;
        }

        /// <summary>
        /// Rotate the 3D model around a custom axis in tridimensional space.
        /// </summary>
        /// <param name="axis">A Vector3 representing the axis to rotate around.</param>
        /// <param name="degrees">The value of the rotation amount (in degrees).</param>
        public void RotateAroundAxis(Vector3 axis, float degrees)
        {
            rotation *= Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(degrees));        // TODO: rotation changing the coordinate system
            updated = false;                                                                        // of the object in a weird way
        }


        /// <summary>
        /// Change the transparency of the object by setting the alpha value of all its meshes.
        /// </summary>
        /// <param name="alpha">The new alpha value for the mesh.</param>
        public void SetAlpha(float alpha)
        {
            if (model3d == null)
                return;

            foreach (ModelMesh mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = alpha;
                }
            }
        }

        /// <summary>
        /// Enables the basic XNA lighting model for all meshes of the object
        /// </summary>
        public void EnableLightingModel()
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
            }
        }


        /// <summary>
        /// Apply all previous changes to the object's Position, Rotation and Scale, updating the ObjectWorldMatrix.
        /// </summary>
        public virtual void Update()
        {
            //if (!active)
                //return;

            // Update objectWorldMatrix
            objectWorldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up)
                * Matrix.CreateScale(scale)
                * Matrix.CreateFromQuaternion(rotation)
                * Matrix.CreateTranslation(position);

            updated = true;
        }


        /// <summary>
        /// Render the object in 3D space, with the previously defined Position, Rotation and Scale.
        /// </summary>
        /// <param name="camera">The camera that will be used to render the object.</param>
        public void Draw(in GameCamera camera)
        {
            if (!active)
                return;

            model3d.Draw(this.ObjectWorldMatrix, camera.View, camera.Projection);
        }

    }
}
