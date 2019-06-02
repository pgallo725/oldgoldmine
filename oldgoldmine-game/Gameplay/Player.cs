using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class Player
    {
        private const float maxVerticalAngle = 15f;
        private const float maxHorizontalAngle = 40f;

        private float verticalLookAngle = 0.0f;
        private float horizontalLookAngle = 0.0f;

        private GameCamera camera;
        private GameObject3D model;
        internal BoundingSphere hitbox;

        public GameCamera Camera { get { return camera; } }

        public Vector3 Position { get { return camera.Position; } }


        public void Initialize(GameCamera playerCamera)
        {
            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position, 1f);
        }

        public void Initialize(GameCamera playerCamera, float hitboxRadius)
        {
            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position, hitboxRadius);
        }

        public void Initialize(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset)
        {
            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableLightingModel();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position, 1f);
        }

        public void Initialize(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset, float hitboxRadius)
        {
            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableLightingModel();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position, hitboxRadius);
        }



        public void RotateUpDown(float degrees)
        {
            camera.RotateViewVertical(degrees);
            // TODO: rotate the model vertically as well
        }

        public void RotateLeftRight(float degrees)
        {
            camera.RotateViewHorizontal(degrees);
            // TODO: rotate the model horizontally as well
        }


        public void LookUpDown(float degrees, bool freeLook)
        {
            float targetAngle = verticalLookAngle + degrees;

            if (!freeLook)
                targetAngle = MathHelper.Clamp(targetAngle, -maxVerticalAngle, maxVerticalAngle);

            camera.RotateViewVertical(targetAngle - verticalLookAngle);
            verticalLookAngle = targetAngle;
        }

        public void LookLeftRight(float degrees, bool freeLook)
        {
            float targetAngle = horizontalLookAngle + degrees;

            if (!freeLook)
                targetAngle = MathHelper.Clamp(targetAngle, -maxHorizontalAngle, maxHorizontalAngle);

            camera.RotateViewHorizontal(targetAngle - horizontalLookAngle);
            horizontalLookAngle = targetAngle;
        }


        public void Move(float speed, Vector3 direction)
        {
            camera.Move(speed * direction);
            if (model != null)
                model.MovePosition(speed * direction);
        }


        public void Update()
        {
            camera.Update();
            hitbox.Center = camera.Position;
        }


        public void Draw()
        {
            if (model != null)
                model.Draw(camera);
        }

    }
}