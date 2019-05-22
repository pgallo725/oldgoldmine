using Microsoft.Xna.Framework;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class Player
    {

        //private readonly Vector3 size = Vector3.One;
        private GameCamera camera;
        //private BoundingBox box;
        internal BoundingSphere hitbox;

        public GameCamera Camera { get { return camera; } }


        public void Initialize(GameCamera playerCamera)
        {
            this.camera = playerCamera;

            this.hitbox = new BoundingSphere(playerCamera.Position, 1f);
        }

        public void Initialize(GameCamera playerCamera, BoundingBox playerHitbox)
        {
            this.camera = playerCamera;
            //this.box = playerHitbox:
        }



        public void LookUpDown(float degrees)
        {
            camera.RotateViewVertical(degrees);
        }

        public void LookLeftRight(float degrees)
        {
            camera.RotateViewHorizontal(degrees);
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