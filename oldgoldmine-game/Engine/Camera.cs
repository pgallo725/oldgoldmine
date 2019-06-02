using System;
using Microsoft.Xna.Framework;


namespace oldgoldmine_game.Engine
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


        // Normalized vectors representing the 6 directions relative to the camera's coordinate system (viewpoint)
        public Vector3 Forward { get { return Vector3.Normalize(viewMatrix.Forward); } }
        public Vector3 Back { get { return Vector3.Normalize(viewMatrix.Backward); } }
        public Vector3 Left { get { return Vector3.Normalize(viewMatrix.Left); } }
        public Vector3 Right { get { return Vector3.Normalize(viewMatrix.Right); } }
        public Vector3 Up { get { return Vector3.Normalize(viewMatrix.Up); } }
        public Vector3 Down { get { return Vector3.Normalize(viewMatrix.Down); } }


        /// <summary>
        /// Setup the Camera object by assigning the position and the coordinates of the initial target,
        /// but also specifying parameters such as FOV, aspect ratio and clipping planes which are
        /// needed to create and initialize the view and projection matrices
        /// </summary>
        public void Initialize(Vector3 camPosition, Vector3 camTarget, float aspectRatio, 
            float fieldOfView = 60f, float clippingPlaneNear = 0.5f, float clippingPlaneFar = 500f)
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
        /// Point the camera to look at the specified position.
        /// </summary>
        /// <param name="targetPosition">The coordinates to look at.</param>
        public void LookAt(Vector3 targetPosition)
        {
            this.target = targetPosition;
            this.direction = target - position;
        }

        /// <summary>
        /// Move the camera in an arbitrary direction in tri-dimensional space, 
        /// it will keep looking in the same direction.
        /// </summary>
        /// <param name="movement">The movement vector.</param>
        public void Move(Vector3 movement)
        {
            this.position += movement;
            this.target += movement;
        }

        /// <summary>
        /// Move the camera in an arbitrary direction in tri-dimensional space, 
        /// the view will be locked on the previous target and therefore the camera will rotate.
        /// </summary>
        /// <param name="movement">The movement vector.</param>
        public void MoveKeepTarget(Vector3 movement)
        {
            this.position += movement;
            this.direction = target - position;
        }


        /// <summary>
        /// Rotate (vertically) the direction in which the camera is looking.
        /// </summary>
        /// <param name="degrees">The amount of rotation, > 0 to look upwards, < 0 to look downwards.</param>
        public void RotateViewVertical(float degrees)               // TODO: limit the vertical camera rotation between minLookDownAngle and maxLookUpAngle
        {
            Matrix t = Matrix.CreateTranslation(-position);
            Quaternion q = Quaternion.CreateFromAxisAngle(this.Right, MathHelper.ToRadians(degrees));

            this.LookAt(Vector3.Transform(target, (t * Matrix.CreateFromQuaternion(q)) * Matrix.Invert(t)));
        }


        /// <summary>
        /// Rotate (horizontally) the direction in which the camera is looking.
        /// </summary>
        /// <param name="degrees">The amount of degrees of the rotation, > 0 to rotate Right, < 0 to rotate Left.</param>
        public void RotateViewHorizontal(float degrees)
        {
            Matrix t = Matrix.CreateTranslation(-position);
            Quaternion q = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.ToRadians(degrees));

            this.LookAt(Vector3.Transform(target, (t * Matrix.CreateFromQuaternion(q)) * Matrix.Invert(t)));
        }
    }
}
