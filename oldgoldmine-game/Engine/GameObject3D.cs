using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Class that represents any entity in the game world that has a 3D model attached to it.
    /// </summary>
    public class GameObject3D : IPoolable
    {
        protected readonly Model model3d;

        protected Vector3 position;
        protected Vector3 scale;
        protected Quaternion rotation;

        /// <summary>
        /// The current position of this object in 3D space.
        /// </summary>
        public virtual Vector3 Position { get { return position; } set { position = value; updated = false; } }

        /// <summary>
        /// The current scale of the GameObject3D model.
        /// </summary>
        public virtual Vector3 Scale { get { return scale; } set { scale = value; updated = false; } }

        /// <summary>
        /// The current rotation of the GameObject3D
        /// </summary>
        public virtual Quaternion Rotation { get { return rotation; } set { rotation = value; updated = false; } }


        private Matrix objectWorldMatrix;
        private bool updated = false;

        /// <summary>
        /// Get the world matrix representing the coordinate system of this object, incorporating any position,
        /// rotation and scale transformation that has been applied to it. Use this for rendering the 3D model.
        /// </summary>
        public Matrix ObjectWorldMatrix
        {
            get
            {
                if (!updated)
                {
                    // Update objectWorldMatrix
                    objectWorldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up)
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateFromQuaternion(rotation)
                        * Matrix.CreateTranslation(position);

                    updated = true;
                }

                return objectWorldMatrix;
            }
        }

        // Normalized vectors representing the 6 directions relative to the object's local coordinate system
        public Vector3 Up { get { return objectWorldMatrix.Up; } }
        public Vector3 Down { get { return objectWorldMatrix.Down; } }
        public Vector3 Left { get { return objectWorldMatrix.Left; } }
        public Vector3 Right { get { return objectWorldMatrix.Right; } }
        public Vector3 Forward { get { return objectWorldMatrix.Forward; } }
        public Vector3 Backward { get { return objectWorldMatrix.Backward; } }


        /// <summary>
        /// Construct an empty GameObject3D, with default position, rotation and size.
        /// </summary>
        public GameObject3D()
            : this(null, Vector3.Zero, Vector3.One, Quaternion.Identity)
        {
        }

        /// <summary>
        /// GameObject3D copy constructor.
        /// </summary>
        /// <param name="other">The GameObject3D to copy from.</param>
        public GameObject3D(GameObject3D other)
            : this(other.model3d, other.position, other.scale, other.rotation)
        {
        }

        /// <summary>
        /// Construct a GameObject3D around an imported Model, with default position, rotation and size.
        /// </summary>
        /// <param name="model">The 3D model of this object.</param>
        public GameObject3D(Model model)
            : this(model, Vector3.Zero, Vector3.One, Quaternion.Identity)
        {
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
        /// <param name="movement">A Vector3 representing the delta movement on each axis.</param>
        public void MovePosition(Vector3 movement)
        {
            this.Position += movement;
        }

        /// <summary>
        /// Change the size of the GameObject3D by scaling its size according to a factor.
        /// </summary>
        /// <param name="factor">Single value representing the scaling factor on all axis.</param>
        public void ScaleSize(float factor)
        {
            this.Scale *= factor;
        }

        /// <summary>
        /// Change the size of the GameObject3D by scaling its size according to a factor.
        /// </summary>
        /// <param name="factor">Vector3 representing the scale factor for each axis.</param>
        public void ScaleSize(Vector3 factor)
        {
            this.Scale *= factor;
        }

        /// <summary>
        /// Rotate the 3D model around a given axis in world space.
        /// </summary>
        /// <param name="axis">A Vector3 representing the axis to rotate around.</param>
        /// <param name="degrees">The value of the rotation amount (in degrees).</param>
        public void RotateAroundAxis(Vector3 axis, float degrees)
        {
            this.Rotation = Quaternion.Concatenate(rotation,
                Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(degrees)));
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
        public void EnableDefaultLighting()
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
        /// Set the color for the ambient lighting pass (XNA default lighting model).
        /// </summary>
        /// <param name="color">The Color to set the ambient light to.</param>
        public void SetAmbientLightColor(Color color)
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.AmbientLightColor = color.ToVector3();
                }
            }
        }

        /// <summary>
        /// Set the color for the diffuse shading pass (XNA default lighting model).
        /// </summary>
        /// <param name="color">The object's materials diffuse color.</param>
        public void SetDiffuseColor(Color color)
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.DiffuseColor = color.ToVector3();
                }
            }
        }

        /// <summary>
        /// Set the color and strength of the specular lighting pass (XNA default lighting model).
        /// </summary>
        /// <param name="color">The object's materials specular color.</param>
        /// <param name="strength">The object's materials specular power.</param>
        public void SetSpecularSettings(Color color, float strength)
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.SpecularColor = color.ToVector3();
                    effect.SpecularPower = strength;
                }
            }
        }

        /// <summary>
        /// Set the color for the emissive shading pass (XNA default lighting model).
        /// </summary>
        /// <param name="color">The object's materials emissive color.</param>
        public void SetEmissiveColor(Color color)
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EmissiveColor = color.ToVector3();
                }
            }
        }

        /// <summary>
        /// Selectively enable or disable up to 3 lights contributing to the object's shading (XNA default lighting model).
        /// </summary>
        /// <param name="lightIndex">Index (0, 1 or 2) of the light that is being accessed.</param>
        /// <param name="enabled">Flag indicating whether the selected light must be enabled or not.</param>
        public void SetLightEnabled(int lightIndex, bool enabled = true)
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    switch (lightIndex)
                    {
                        case 0:
                            effect.DirectionalLight0.Enabled = enabled;
                            break;

                        case 1:
                            effect.DirectionalLight1.Enabled = enabled;
                            break;

                        case 2:
                            effect.DirectionalLight2.Enabled = enabled;
                            break;

                        default:
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// Set the main properties for up to 3 lights contributing to the object's shading (XNA default lighting model).
        /// </summary>
        /// <param name="lightIndex">Index (0, 1 or 2) of the light that is being accessed.</param>
        /// <param name="diffuseColor">Diffuse color that the selected light will use.</param>
        /// <param name="specularColor">Specular color that the selected light will use.</param>
        /// <param name="direction">Set the direction of the selected light.</param>
        public void SetLightProperties(int lightIndex, Color diffuseColor, Color specularColor, Vector3 direction)
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    switch (lightIndex)
                    {
                        case 0:
                            effect.DirectionalLight0.DiffuseColor = diffuseColor.ToVector3();
                            effect.DirectionalLight0.SpecularColor = specularColor.ToVector3();
                            effect.DirectionalLight0.Direction = direction;
                            break;

                        case 1:
                            effect.DirectionalLight1.DiffuseColor = diffuseColor.ToVector3();
                            effect.DirectionalLight1.SpecularColor = specularColor.ToVector3();
                            effect.DirectionalLight1.Direction = direction;
                            break;

                        case 2:
                            effect.DirectionalLight2.DiffuseColor = diffuseColor.ToVector3();
                            effect.DirectionalLight2.SpecularColor = specularColor.ToVector3();
                            effect.DirectionalLight2.Direction = direction;
                            break;

                        default:
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// Set properties for the fogging effect applied to distant objects.
        /// </summary>
        /// <param name="enabled">Toggle the rendering of distance fog on/off.</param>
        /// <param name="fogColor">The color of the rendered fog.</param>
        /// <param name="fogStart">The world space distance from the camera at which fogging begins.</param>
        /// <param name="fogEnd">The distance from the camera at which fogging is fully applied (should be > fogStart).</param>
        public void SetFogEffect(bool enabled, Color fogColor, float fogStart, float fogEnd)
        {
            if (model3d == null)
                return;

            foreach (var mesh in model3d.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.FogEnabled = enabled;
                    effect.FogColor = fogColor.ToVector3();
                    effect.FogStart = fogStart;
                    effect.FogEnd = fogEnd;
                }
            }
        }


        /// <summary>
        /// Render the object in 3D space, with the previously defined Position, Rotation and Scale.
        /// </summary>
        /// <param name="camera">The camera that will be used to render the object.</param>
        public virtual void Draw(in GameCamera camera)
        {
            if (IsActive && model3d != null)
                model3d.Draw(this.ObjectWorldMatrix, camera.View, camera.Projection);
        }


        /// <summary>
        /// Implementation of the Clone() method for the IPoolable interface.
        /// </summary>
        /// <returns>A new GameObject3D that is an exact copy of this one.</returns>
        public override object Clone()
        {
            return new GameObject3D(this.model3d, this.position, this.scale, this.rotation);
        }
    }
}
