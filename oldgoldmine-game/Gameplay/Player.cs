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
        

        private enum AnimationState
        {
            Idle,
            LeftMovement,
            LeftMovementReverse,
            RightMovement,
            RightMovementReverse,
            Jump,
            Crouch,
            CrouchReverse
        }

        private const float jumpHeight = 8f;
        private const float jumpVelocity = 15f;
        private readonly float jumpAcceleration;
        private readonly float jumpDuration;

        private float jumpTimepoint = 0f;
        private float currentJumpPosition = 0f;


        private readonly float rightMovementOffset = -1.5f;
        private readonly float rightMovementRotationOffset = -15f;
        private readonly float rightMovementAnimDuration = 0.25f;
        private float rightMovementTimepoint = 0f;
        private float currentRotation = 0f;
        private float currentRightMovement = 0f;

        private AnimationState state = AnimationState.Idle;

        private Vector3 straightPlayerPosition;


        public Player()
        {
            jumpDuration = ((float)2/3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = - (jumpVelocity / jumpDuration);
        }

        public void Initialize(GameCamera playerCamera)
        {
            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position, 1f);

            this.straightPlayerPosition = playerCamera.Position;
        }

        public void Initialize(GameCamera playerCamera, float hitboxRadius)
        {
            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position, hitboxRadius);

            this.straightPlayerPosition = playerCamera.Position;
        }

        public void Initialize(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset)
        {
            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableLightingModel();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position, 1f);

            this.straightPlayerPosition = playerCamera.Position;
        }

        public void Initialize(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset, float hitboxRadius)
        {
            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableLightingModel();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position, hitboxRadius);

            this.straightPlayerPosition = playerCamera.Position;
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


        public void Update(GameTime gameTime)
        {
            this.UpdateJump(gameTime);
            camera.Update();
            hitbox.Center = camera.Position;
        }


        public void Draw()
        {
            if (model != null)
                model.Draw(camera);
        }

        

        public void UpdateRightMovement(GameTime gameTime)
        {
            if (state == AnimationState.Idle)
                state = AnimationState.RightMovement;

            if (state == AnimationState.RightMovement)
            {
                rightMovementTimepoint = MathHelper.Clamp(
                rightMovementTimepoint + (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, rightMovementAnimDuration);

                float interpolatedRotation = MathHelper.Lerp(0f, rightMovementRotationOffset,
                    rightMovementTimepoint / rightMovementAnimDuration);

                float interpolatedRightMovement = MathHelper.Lerp(0f, rightMovementOffset,
                    rightMovementTimepoint / rightMovementAnimDuration);

                if (model != null)
                {
                    model.RotateAroundAxis(Vector3.Forward, interpolatedRotation - currentRotation);
                    model.MovePosition(Vector3.Right * (interpolatedRightMovement - currentRightMovement));
                }

                camera.Move(Vector3.Right * (interpolatedRightMovement - currentRightMovement));

                currentRotation = interpolatedRotation;
                currentRightMovement = interpolatedRightMovement;
            }
            else if (state == AnimationState.RightMovementReverse)
            {
                rightMovementTimepoint = MathHelper.Clamp(
                rightMovementTimepoint + (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, rightMovementAnimDuration);

                float interpolatedRotation = MathHelper.Lerp(rightMovementOffset, 0f,
                    rightMovementTimepoint / rightMovementAnimDuration);

                float interpolatedRightMovement = MathHelper.Lerp(rightMovementOffset, 0f,
                    rightMovementTimepoint / rightMovementAnimDuration);

                if (model != null)
                {
                    model.RotateAroundAxis(Vector3.Forward, interpolatedRotation - currentRotation);
                    model.MovePosition(Vector3.Right * (interpolatedRightMovement - currentRightMovement));
                }

                camera.Move(Vector3.Right * (interpolatedRightMovement - currentRightMovement));

                currentRotation = interpolatedRotation;
                currentRightMovement = interpolatedRightMovement;

                if (rightMovementTimepoint == rightMovementAnimDuration)
                {
                    state = AnimationState.Idle;
                    rightMovementTimepoint = 0f;
                }
            }
        }


        public void Jump()
        {
            if (state == AnimationState.Idle)
            {
                state = AnimationState.Jump;
                jumpTimepoint = 0f;
            }
        }


        private void UpdateJump(GameTime gameTime)
        {
            if (state == AnimationState.Jump)
            {
                jumpTimepoint += (float)gameTime.ElapsedGameTime.TotalSeconds;

                float verticalPosition = jumpVelocity * jumpTimepoint +
                    jumpAcceleration * (jumpTimepoint * jumpTimepoint) / 2f;

                float interpolatedJumpPosition = MathHelper.Clamp(verticalPosition, 0f, float.MaxValue);

                if (model != null)
                    model.MovePosition(Vector3.Up * (interpolatedJumpPosition - currentJumpPosition));

                camera.Move(Vector3.Up * (interpolatedJumpPosition - currentJumpPosition));

                currentJumpPosition = interpolatedJumpPosition;

                if (interpolatedJumpPosition == 0f)
                    state = AnimationState.Idle;        // Stop jump animation when the rail is hit again
            }
        }
    }
}