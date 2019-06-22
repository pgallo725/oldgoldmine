using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class Player
    {
        private const float maxVerticalAngle = 12f;
        private const float maxHorizontalAngle = 33f;

        private float verticalLookAngle = 0.0f;
        private float horizontalLookAngle = 0.0f;

        private GameCamera camera;
        private GameObject3D model;

        private readonly Vector3 hitboxOffset = new Vector3(0f, 0f, -0.5f);
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

        // Current animation state
        private AnimationState state = AnimationState.Idle;


        // Jump animation parameters
        private const float jumpHeight = 9f;
        private const float jumpVelocity = 16f;
        private const float jumpCooldown = 0.2f;
        private readonly float jumpAcceleration;
        private readonly float jumpDuration;
        

        // Jump animation status variables
        private float jumpTimepoint = 0f;
        private float currentJumpPosition = 0f;
        private float timeBeforeNextJump = 0f;


        // Side movement animation parameters
        private const float sideMovementOffset = -1.5f;
        private const float sideMovementRotationOffset = -15f;
        private const float sideMovementAnimDuration = 0.2f;

        // Side movement animation status variables
        private float sideMovementTimepoint = 0f;
        private float currentRotation = 0f;
        private float currentSidePosition = 0f;


        // Crouch animation parameters
        private const float crouchAnimDuration = 0.2f;
        private const float crouchVerticalPosition = 1.75f;
        private const float normalVerticalPosition = 2.5f;

        // Crouch animation status variables
        private float crouchTimepoint = 0f;
        private float currentVerticalPosition = normalVerticalPosition;


        public Player(GameCamera playerCamera)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, 1f);
        }

        public Player(GameCamera playerCamera, float hitboxRadius)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, hitboxRadius);
        }

        public Player(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableLightingModel();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, 1f);
        }

        public Player(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset, float hitboxRadius)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableLightingModel();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, hitboxRadius);
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
            timeBeforeNextJump = MathHelper.Clamp(
                timeBeforeNextJump - (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, float.MaxValue);

            this.UpdateJump(gameTime);

            // Update "automatic" animations 
            // (reversing previous movements when the key is released)
            if (state == AnimationState.RightMovementReverse)
                ReverseSideMovement(gameTime,Vector3.Right);
            else if (state == AnimationState.LeftMovementReverse)
                ReverseSideMovement(gameTime, Vector3.Left);
            else if (state == AnimationState.CrouchReverse)
                UpdateCrouchMovement(gameTime);

            camera.Update();
            hitbox.Center = camera.Position + hitboxOffset;
        }


        public void Draw()
        {
            if (model != null)
                model.Draw(camera);
        }

        

        public void UpdateSideMovement(GameTime gameTime, Vector3 direction)
        {
            if (state == AnimationState.Idle && direction == Vector3.Right)
                state = AnimationState.RightMovement;
            else if (state == AnimationState.Idle && direction == Vector3.Left)
                state = AnimationState.LeftMovement;

            if ((state == AnimationState.RightMovement && direction == Vector3.Right) ||
                (state == AnimationState.LeftMovement && direction == Vector3.Left))
            {
                sideMovementTimepoint = MathHelper.Clamp(
                sideMovementTimepoint + (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, sideMovementAnimDuration);

                float interpolatedRotation = MathHelper.Lerp(0f, sideMovementRotationOffset,
                    sideMovementTimepoint / sideMovementAnimDuration);

                float interpolatedPosition = MathHelper.Lerp(0f, sideMovementOffset,
                    sideMovementTimepoint / sideMovementAnimDuration);

                if (model != null)
                {
                    model.RotateAroundAxis(Vector3.Forward, direction.X * (interpolatedRotation - currentRotation));
                    model.MovePosition(direction * (interpolatedPosition - currentSidePosition));
                }

                camera.Move(direction * (interpolatedPosition - currentSidePosition));

                currentRotation = interpolatedRotation;
                currentSidePosition = interpolatedPosition;
            }
        }

        private void ReverseSideMovement(GameTime gameTime, Vector3 direction)
        {
            if ((state == AnimationState.RightMovementReverse && direction == Vector3.Right) ||
                (state == AnimationState.LeftMovementReverse && direction == Vector3.Left))
            {
                sideMovementTimepoint = MathHelper.Clamp(
                sideMovementTimepoint + (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, sideMovementAnimDuration);

                float interpolatedRotation = MathHelper.Lerp(sideMovementRotationOffset, 0f,
                    sideMovementTimepoint / sideMovementAnimDuration);

                float interpolatedPosition = MathHelper.Lerp(sideMovementOffset, 0f,
                    sideMovementTimepoint / sideMovementAnimDuration);

                if (model != null)
                {
                    model.RotateAroundAxis(Vector3.Forward, direction.X * (interpolatedRotation - currentRotation));
                    model.MovePosition(direction * (interpolatedPosition - currentSidePosition));
                }

                camera.Move(direction * (interpolatedPosition - currentSidePosition));

                currentRotation = interpolatedRotation;
                currentSidePosition = interpolatedPosition;

                if (sideMovementTimepoint == sideMovementAnimDuration)
                {
                    state = AnimationState.Idle;
                    sideMovementTimepoint = 0f;
                }
            }
        }


        public void ReverseSideMovement(Vector3 reverseDirection)
        {
            if (state == AnimationState.RightMovement && reverseDirection == Vector3.Left)
            {
                state = AnimationState.RightMovementReverse;
                sideMovementTimepoint = sideMovementAnimDuration - sideMovementTimepoint;
            }
            else if (state == AnimationState.LeftMovement && reverseDirection == Vector3.Right)
            {
                state = AnimationState.LeftMovementReverse;
                sideMovementTimepoint = sideMovementAnimDuration - sideMovementTimepoint;
            }
        }



        public void UpdateCrouchMovement(GameTime gameTime)
        {
            if (state == AnimationState.Idle)
                state = AnimationState.Crouch;

            if (state == AnimationState.Crouch)
            {
                crouchTimepoint = MathHelper.Clamp(
                crouchTimepoint + (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, crouchAnimDuration);

                float interpolatedPosition = MathHelper.Lerp(normalVerticalPosition,
                    crouchVerticalPosition, crouchTimepoint / crouchAnimDuration);

                camera.Move(Vector3.Up * (interpolatedPosition - currentVerticalPosition));

                currentVerticalPosition = interpolatedPosition;
            }
            else if (state == AnimationState.CrouchReverse)
            {
                crouchTimepoint = MathHelper.Clamp(
                crouchTimepoint + (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, crouchAnimDuration);

                float interpolatedPosition = MathHelper.Lerp(crouchVerticalPosition,
                    normalVerticalPosition, crouchTimepoint / crouchAnimDuration);

                camera.Move(Vector3.Up * (interpolatedPosition - currentVerticalPosition));

                currentVerticalPosition = interpolatedPosition;

                if (currentVerticalPosition == normalVerticalPosition)
                {
                    state = AnimationState.Idle;
                    crouchTimepoint = 0f;
                }
            }
        }


        public void ReverseCrouch()
        {
            if (state == AnimationState.Crouch)
            {
                state = AnimationState.CrouchReverse;
                crouchTimepoint = 0f;
            }
        }



        public void Jump()
        {
            if (state == AnimationState.Idle && timeBeforeNextJump == 0f)
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
                {
                    state = AnimationState.Idle;        // Stop jump animation when the rail is hit again
                    timeBeforeNextJump = jumpCooldown;
                }
            }
        }
    }
}