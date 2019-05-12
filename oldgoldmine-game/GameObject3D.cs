using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace oldgoldmine_game
{

    class GameObject3D
    {
        private readonly Model model3d;

        private Vector3 position;
        private Vector3 scale;
        private Quaternion rotation;


        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Scale { get { return scale; } set { scale = value; } }
        public Quaternion Rotation { get { return rotation; } set { rotation = value; } }

        private Matrix objectWorldMatrix;
        private bool updated = false;

        public Matrix ObjectWorldMatrix { get { if (!updated) this.Update(); return objectWorldMatrix; } }


        public GameObject3D()
        {
            this.model3d = null;
            this.position = Vector3.Zero;
            this.scale = Vector3.One;
            this.rotation = Quaternion.Identity;
        }

        public GameObject3D(Model model)
        {
            this.model3d = model;
            this.position = Vector3.Zero;
            this.scale = Vector3.One;
            this.rotation = Quaternion.Identity;
        }

        public GameObject3D(Model model, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            this.model3d = model;
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }


        public void MovePosition(Vector3 movement)
        {
            position += movement;
            updated = false;
        }

        public void ScaleSize(float scale)
        {
            this.scale.X = scale;
            this.scale.Y = scale;
            this.scale.Z = scale;
            updated = false;
        }

        public void ScaleSize(float scaleX = 1f, float scaleY = 1f, float scaleZ = 1f)
        {
            scale.X = scaleX;
            scale.Y = scaleY;
            scale.Z = scaleZ;
            updated = false;
        }

        public void ScaleSize(Vector3 scale)
        {
            this.scale = scale;
            updated = false;
        }

        public void RotateAroundAxis(Vector3 axis, float degrees)
        {
            rotation *= Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(degrees));
            updated = false;
        }


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


        public void Draw(in GameCamera camera)
        {
            model3d.Draw(this.ObjectWorldMatrix, camera.View, camera.Projection);
        }


        public void Update()
        {
            // Update objectWorldMatrix
            objectWorldMatrix = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up)
                * Matrix.CreateScale(scale)
                * Matrix.CreateFromQuaternion(rotation)
                * Matrix.CreateTranslation(position);

            updated = true;
        }
    }
}
