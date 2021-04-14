using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using oldgoldmine_game.Engine;


namespace oldgoldmine_game.Gameplay
{
    public class Player
    {
        // Maximum angles allowed for looking around vertically or horizontally
        private const float maxVerticalAngle = 15f;
        private const float maxHorizontalAngle = 40f;

        // Current view angles (relative to movement direction, or Z axis)
        private float verticalLookAngle = 0.0f;
        private float horizontalLookAngle = 0.0f;

        private GameCamera camera;
        private GameObject3D model;
        private SoundEffectInstance sound;

        private readonly Vector3 hitboxOffset = new Vector3(0f, -0.5f, 0f);
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
        private const float jumpCooldown = 0.1f;
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



        /* Constructors first calculate jump animation values based on configuration parameters,
         * then initialize AudioManager so that it can play the proper sound effects and 
         * finally creates the Player object by putting together a Camera, a 3D model and an hitbox */

        public Player(GameCamera playerCamera)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            sound = AudioManager.PlaySoundEffect("Minecart_Loop", true, 0.8f, 0.2f);
            sound.Pause();

            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, 1f);
        }

        public Player(GameCamera playerCamera, float hitboxRadius)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            sound = AudioManager.PlaySoundEffect("Minecart_Loop", true, 0.8f, 0.2f);
            sound.Pause();

            this.camera = playerCamera;
            this.model = null;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, hitboxRadius);
        }

        public Player(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            sound = AudioManager.PlaySoundEffect("Minecart_Loop", true, 0.8f, 0.2f);
            sound.Pause();

            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableDefaultLighting();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, 1f);
        }

        public Player(GameCamera playerCamera, GameObject3D playerModel, Vector3 modelOffset, float hitboxRadius)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            sound = AudioManager.PlaySoundEffect("Minecart_Loop", true, 0.8f, 0.2f);
            sound.Pause();

            this.camera = playerCamera;
            this.model = playerModel;
            this.model.EnableDefaultLighting();
            this.model.Position = playerCamera.Position + modelOffset;
            this.hitbox = new BoundingSphere(playerCamera.Position + hitboxOffset, hitboxRadius);
        }


        // Rotate the whole player (model + view) horizontally or vertically

        public void RotateUpDown(float degrees)
        {
            camera.RotateViewVertical(degrees);
            model.RotateAroundAxis(Vector3.Left, degrees);
        }

        public void RotateLeftRight(float degrees)
        {
            camera.RotateViewHorizontal(degrees);
            model.RotateAroundAxis(Vector3.Down, degrees);
        }


        // Rotate the player's view horizontally or vertically,
        // constraining it to the maxVerticalAngle and maxHorizontalAngle values

        public void LookUpDown(float degrees, bool freeMovement = false)
        {
            float targetAngle = verticalLookAngle + degrees;

            if (!freeMovement)
                targetAngle = MathHelper.Clamp(targetAngle, -maxVerticalAngle, maxVerticalAngle);

            camera.RotateViewVertical(targetAngle - verticalLookAngle);
            verticalLookAngle = targetAngle;
        }

        public void LookLeftRight(float degrees, bool freeMovement = false)
        {
            float targetAngle = horizontalLookAngle + degrees;

            if (!freeMovement)
                targetAngle = MathHelper.Clamp(targetAngle, -maxHorizontalAngle, maxHorizontalAngle);

            camera.RotateViewHorizontal(targetAngle - horizontalLookAngle);
            horizontalLookAngle = targetAngle;
        }


        // Fix the player's camera view back to the center 
        // (aligned with movement direction on Z axis)
        public void ResetCameraLook()
        {
            LookUpDown(-verticalLookAngle, true);
            verticalLookAngle = 0f;

            LookLeftRight(-horizontalLookAngle, true);
            horizontalLookAngle = 0f;
        }


        public void Move(float speed, Vector3 direction)
        {
            camera.Move(speed * direction);
            if (model != null)
                model.MovePosition(speed * direction);
        }


        // Handle game state changes relative to the player

        public void Start()
        {
            sound.Play();
        }

        public void Pause()
        {
            sound.Pause();
        }

        public void Resume()
        {
            sound.Resume();
        }

        public void Kill()
        {
            sound.Stop();
            AudioManager.StopSoundEffect("Gold_Pickup");
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



        /* Based on the type of side movement animation, the player model and camera position are
         * interpolated between the 2 keyframe positions according to the current animation timepoint */

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

                // Adjust audio panning towards the animation side direction, to emphasize the movement
                float interpolatedPannning = MathHelper.Lerp(0f, direction == Vector3.Left ? -0.33f : 0.33f,
                    sideMovementTimepoint / sideMovementAnimDuration);

                if (model != null)
                {
                    model.RotateAroundAxis(Vector3.Forward, direction.X * (interpolatedRotation - currentRotation));
                    model.MovePosition(direction * (interpolatedPosition - currentSidePosition));
                }

                camera.Move(direction * (interpolatedPosition - currentSidePosition));
                sound.Pan = interpolatedPannning;

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

                // Gradually bring the audio panning back to centered
                float interpolatedPannning = MathHelper.Lerp(direction == Vector3.Left ? -0.33f : 0.33f, 0f,
                    sideMovementTimepoint / sideMovementAnimDuration);

                if (model != null)
                {
                    model.RotateAroundAxis(Vector3.Forward, direction.X * (interpolatedRotation - currentRotation));
                    model.MovePosition(direction * (interpolatedPosition - currentSidePosition));
                }

                camera.Move(direction * (interpolatedPosition - currentSidePosition));
                sound.Pan = interpolatedPannning;

                currentRotation = interpolatedRotation;
                currentSidePosition = interpolatedPosition;

                if (sideMovementTimepoint == sideMovementAnimDuration)
                {
                    state = AnimationState.Idle;
                    sideMovementTimepoint = 0f;
                }
            }
        }


        // Start the animation of returning from a left or right position
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

            // Based on the direction of the animation (crouching or standing up again)
            // the vertical position of the camera is obtained by interpolating the 2 keyframe positions
            // according to the current animation timepoint

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


        // Start the animation of returning from the crouch position
        public void ReverseCrouch()
        {
            if (state == AnimationState.Crouch)
            {
                state = AnimationState.CrouchReverse;
                crouchTimepoint = 0f;
            }
        }


        // Start the jumping animation
        public void Jump()
        {
            if (state == AnimationState.Idle && timeBeforeNextJump == 0f)
            {
                sound.Pause();
                state = AnimationState.Jump;
                jumpTimepoint = 0f;
            }
        }


        // While the jumping animation is active, update at every frame the vertical position
        // according to the physical law of x(t) = v0 * t + 1/2 * a * t^2
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

                // At the end of the jumping animation, play the Rails_Hit sound effect
                if (interpolatedJumpPosition == 0f)
                {
                    sound.Resume();
                    AudioManager.PlaySoundEffect("Rails_Hit", false, 0.6f, -0.1f);

                    state = AnimationState.Idle;            // Stop jump animation when the rail is hit again
                    timeBeforeNextJump = jumpCooldown;
                }
            }
        }
    }
}