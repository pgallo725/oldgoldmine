using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace oldgoldmine_game
{
    public class GameCamera
    {
        private Vector3 target;
        private Vector3 position;
        private Vector3 direction;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        public Matrix View { get { return viewMatrix; } private set { viewMatrix = value; } }
        public Matrix Projection { get { return projectionMatrix; } }


        public Vector3 Position { get { return position; } private set { } }
        public Vector3 Target { get { return target; } set { LookAt(value); } }
        
        public Vector3 Forward { get { return Vector3.Normalize(direction); } }
        public Vector3 Back { get { return Vector3.Normalize(-direction); } }
        public Vector3 Left { get { return Vector3.Normalize(new Vector3(direction.Z, 0f, -direction.X)); } }
        public Vector3 Right { get { return Vector3.Normalize(new Vector3(-direction.Z, 0f, direction.X)); } }
        public Vector3 Up { get { return Vector3.Normalize(new Vector3(direction.X, direction.Z, -direction.Y)); } }
        public Vector3 Down { get { return Vector3.Normalize(new Vector3(-direction.X, -direction.Z, direction.Y)); } }

        



        /// <summary>
        /// Setup the Camera object by assigning the position and the coordinates of the initial target,
        /// but also specifying parameters such as FOV, aspect ratio and clipping planes which are
        /// needed to create and initialize the view and projection matrices
        /// </summary>
        public void Initialize(Vector3 camPosition, Vector3 camTarget, float aspectRatio, 
            float fieldOfView = 60f, float clippingPlaneNear = 1f, float clippingPlaneFar = 100f)
        {
            this.position = camPosition;
            this.target = camTarget;
            this.direction = camTarget - camPosition;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(fieldOfView), aspectRatio,
                clippingPlaneNear, clippingPlaneFar);

            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
        }


        /// <summary>
        /// Updates the camera's ViewMatrix with the latest position, rotation and target informations
        /// NOTE: always call this method at the end of Game.Update() before Draw() begins
        /// </summary>
        public void Update()
        {
            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
        }

        /// <summary>
        /// Point the camera to look at the specified position in tri-dimensional space
        /// </summary>
        public void LookAt(Vector3 targetPosition)
        {
            this.target = targetPosition;
            this.direction = target - position;
        }

        public void Move(float speed, Vector3 direction)
        {
            this.position += speed * direction;
            this.target += speed * direction;
        }

        public void MoveKeepTarget(float speed, Vector3 direction)
        {
            this.position += speed * direction;
            this.direction = target - position;
        }


        public void RotateX(float degrees)
        {

        }

        public void RotateY(float degrees)
        {

        }

        public void RotateZ(float degrees)
        {

        }

        public void RotateAroundTargetY(float degrees)
        {
            this.position = Vector3.Transform(position - target,
                Matrix.CreateRotationY(MathHelper.ToRadians(degrees))) + target;

            this.direction = target - position;
        }
    }
}
