using Microsoft.Xna.Framework;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Camera class, responsible for updating and holding the View and Projection matrices used to render the scene.
    /// </summary>
    public class GameCamera
    {
        private Vector3 target;
        private Vector3 position;

        /// <summary>
        /// The current position of the Camera in 3D space.
        /// </summary>
        public Vector3 Position { get { return position; } set { position = value; } }

        /// <summary>
        /// The position of the target which this Camera is currently looking at.
        /// </summary>
        public Vector3 Target { get { return target; } set { target = value; LookAt(target); } }

        /// <summary>
        /// The view matrix of the Camera for the current frame.
        /// NOTE: if Update() hasn't been called yet, it refers to the previous frame.
        /// </summary>
        public Matrix View { get; private set; }

        /// <summary>
        /// The projection matrix generated for this Camera (based on FOV, aspect ratio and clipping planes).
        /// </summary>
        public readonly Matrix Projection;

        // Normalized vectors representing the 6 directions relative to the Camera's coordinate system (viewpoint)
        public Vector3 Forward { get { return Vector3.Normalize(View.Forward); } }
        public Vector3 Back { get { return Vector3.Normalize(View.Backward); } }
        public Vector3 Left { get { return Vector3.Normalize(View.Left); } }
        public Vector3 Right { get { return Vector3.Normalize(View.Right); } }
        public Vector3 Up { get { return Vector3.Normalize(View.Up); } }
        public Vector3 Down { get { return Vector3.Normalize(View.Down); } }


        /// <summary>
        /// Create a GameCamera object by specifying parameters such as FOV, aspect ratio and clipping planes
        /// which are needed to create and initialize the view and projection matrices.
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio of the camera frustum, expressed as a float value (e.g. 4:3 = 1,333).</param>
        /// <param name="fieldOfView">Field of view of the camera, in degrees.</param>
        /// <param name="clippingPlaneNear">Distance of the near clipping plane.</param>
        /// <param name="clippingPlaneFar">Distance of the far clipping plane.</param>
        public GameCamera(float aspectRatio, float fieldOfView = 60f, float clippingPlaneNear = 0.5f, float clippingPlaneFar = 500f)
        {
            this.position = Vector3.Zero;
            this.target = position + Vector3.Forward;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView),
                aspectRatio, clippingPlaneNear, clippingPlaneFar);

            View = Matrix.CreateLookAt(position, target, Vector3.Up);
        }


        /// <summary>
        /// Updates the camera's ViewMatrix with the latest position, rotation and target informations
        /// NOTE: always call this method at the end of Game.Update() before Draw() begins
        /// </summary>
        public void Update()
        {
            View = Matrix.CreateLookAt(position, target, Vector3.Up);
        }


        /// <summary>
        /// Point the camera to look at the specified position.
        /// </summary>
        /// <param name="targetPosition">The coordinates to look at.</param>
        public void LookAt(Vector3 targetPosition)
        {
            this.target = targetPosition;
        }

        /// <summary>
        /// Move the camera in an arbitrary direction in tri-dimensional space, 
        /// it will keep looking in the same direction.
        /// </summary>
        /// <param name="movement">The movement vector.</param>
        /// <param name="keepTarget">Whether the view has to be locked to the previous
        /// target or move along with the camera.</param>
        public void Move(Vector3 movement, bool keepTarget = false)
        {
            this.position += movement;
            if (!keepTarget)
                this.target += movement;
        }


        /// <summary>
        /// Rotate (vertically) the direction in which the camera is looking.
        /// </summary>
        /// <param name="degrees">The rotation amount, in degrees. Use > 0 to look upwards, < 0 to look downwards.</param>
        public void RotateViewVertical(float degrees)
        {
            Matrix t = Matrix.CreateTranslation(-position);
            Quaternion q = Quaternion.CreateFromAxisAngle(this.Right, MathHelper.ToRadians(degrees));

            this.LookAt(Vector3.Transform(target, t * Matrix.CreateFromQuaternion(q) * Matrix.Invert(t)));
        }

        /// <summary>
        /// Rotate (horizontally) the direction in which the camera is looking.
        /// </summary>
        /// <param name="degrees">The rotation amount, in degrees. Use > 0 to rotate Right, < 0 to rotate Left.</param>
        public void RotateViewHorizontal(float degrees)
        {
            Matrix t = Matrix.CreateTranslation(-position);
            Quaternion q = Quaternion.CreateFromAxisAngle(this.Down, MathHelper.ToRadians(degrees));

            this.LookAt(Vector3.Transform(target, t * Matrix.CreateFromQuaternion(q) * Matrix.Invert(t)));
        }
    }
}
