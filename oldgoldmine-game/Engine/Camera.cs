using Microsoft.Xna.Framework;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Camera class, responsible for updating and holding the View and Projection matrices used to render the scene.
    /// </summary>
    public class Camera
    {
        private Vector3 position;
        private Vector3 lookAt;

        /// <summary>
        /// The current position of the Camera in 3D space.
        /// </summary>
        public Vector3 Position { get { return position; } set { Move(value - position); } }


        private Matrix viewMatrix;
        private bool updated = false;

        /// <summary>
        /// The view matrix of the Camera for the current frame, incorporating the latest
        /// position and rotation values applied to the Camera object.
        /// </summary>
        public Matrix View
        {
            get
            {
                if (!updated)
                {
                    // Update viewMatrix
                    viewMatrix = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
                    updated = true;
                }

                return viewMatrix;
            }
        }

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
        public Camera(float aspectRatio, float fieldOfView = 60f, float clippingPlaneNear = 0.5f, float clippingPlaneFar = 500f)
        {
            this.position = Vector3.Zero;
            this.lookAt = position + Vector3.UnitZ;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView),
                aspectRatio, clippingPlaneNear, clippingPlaneFar);
        }


        /// <summary>
        /// Move the camera in an arbitrary direction in tri-dimensional space.
        /// </summary>
        /// <param name="movement">The movement vector.</param>
        public void Move(Vector3 movement)
        {
            this.position += movement;
            this.lookAt += movement;

            updated = false;
        }

        /// <summary>
        /// Point the camera to look at the specified position.
        /// </summary>
        /// <param name="targetPosition">The coordinates to look at.</param>
        public void LookAt(Vector3 targetPosition)
        {
            this.lookAt = targetPosition;

            updated = false;
        }


        /// <summary>
        /// Set the Camera rotation using a set of yaw, pitch and roll values.
        /// </summary>
        /// <param name="yaw">Yaw (rotation around vertical axis), in radians.</param>
        /// <param name="pitch">Pitch (rotation around horizontal axis), in radians.</param>
        /// <param name="roll">Roll (rotation forward axis), in radians.</param>
        public void SetRotation(float yaw, float pitch, float roll)
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotation);
            lookAt = position + lookAtOffset;

            updated = false;
        }

        /// <summary>
        /// Set the Camera rotation using a tri-dimensional vector.
        /// </summary>
        /// <param name="vRotation">Vector3 containing the rotation of the camera around each axis, in radians.</param>
        public void SetRotation(Vector3 vRotation)
        {
            Matrix rotation = Matrix.CreateRotationX(vRotation.X) *
                Matrix.CreateRotationX(vRotation.Y) *
                Matrix.CreateRotationX(vRotation.Z);

            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotation);
            lookAt = position + lookAtOffset;

            updated = false;
        }

        /// <summary>
        /// Set the Camera rotation using a quaternion.
        /// </summary>
        /// <param name="qRotation">Quaternion representing the desired Camera rotation.</param>
        public void SetRotation(Quaternion qRotation)
        {
            Matrix rotation = Matrix.CreateFromQuaternion(qRotation);

            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotation);
            lookAt = position + lookAtOffset;

            updated = false;
        }

    }
}
