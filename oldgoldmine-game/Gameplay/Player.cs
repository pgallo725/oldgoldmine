using Microsoft.Xna.Framework;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class Player
    {
        private const float maxVerticalAngle = 40f;
        private const float maxHorizontalAngle = 45f;

        private float verticalLookAngle = 0.0f;
        private float horizontalLookAngle = 0.0f;

        private GameCamera camera;
        internal BoundingSphere hitbox;

        public GameCamera Camera { get { return camera; } }


        public void Initialize(GameCamera playerCamera)
        {
            this.camera = playerCamera;
            this.hitbox = new BoundingSphere(playerCamera.Position, 1f);
        }

        public void Initialize(GameCamera playerCamera, float hitboxRadius)
        {
            this.camera = playerCamera;
            this.hitbox = new BoundingSphere(playerCamera.Position, hitboxRadius);
        }



        public void RotateUpDown(float degrees)
        {
            camera.RotateViewVertical(degrees);
        }

        public void RotateLeftRight(float degrees)
        {
            camera.RotateViewHorizontal(degrees);
        }

        public void LookUpDown(float degrees)
        {
            float targetAngle = MathHelper.Clamp(verticalLookAngle + degrees,
                -maxVerticalAngle, maxVerticalAngle);

            camera.RotateViewVertical(targetAngle - verticalLookAngle);
            verticalLookAngle = targetAngle;
        }

        public void LookLeftRight(float degrees)
        {
            float targetAngle = MathHelper.Clamp(horizontalLookAngle + degrees,
                -maxHorizontalAngle, maxHorizontalAngle);

            camera.RotateViewHorizontal(targetAngle - horizontalLookAngle);
            horizontalLookAngle = targetAngle;
        }

        public void Move(float speed, Vector3 direction)
        {
            camera.Move(speed * direction);
        }

        public void Update()
        {
            camera.Update();
            hitbox.Center = camera.Position;
        }

    }
}