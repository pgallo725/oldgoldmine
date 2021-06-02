using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using OldGoldMine.Engine;


namespace OldGoldMine.Gameplay
{
    public class Player
    {
        // Maximum angles allowed for looking around vertically or horizontally
        private const float maxVerticalAngle = 20f;
        private const float maxHorizontalAngle = 45f;

        // Current view angles (relative to movement direction, or Z axis)
        private float verticalLookAngle = 0.0f;
        private float horizontalLookAngle = 0.0f;

        bool freeLook = false;

        private readonly GameCamera camera;
        private readonly GameObject3D model;
        private readonly SoundEffectInstance sound;

        private BoundingSphere hitbox;
        private Vector3 hitboxOffset;

        /// <summary>
        /// The camera object through which the Player observes the game world.
        /// </summary>
        public GameCamera Camera { get { return camera; } }

        /// <summary>
        /// The current position of the Player object in the game world.
        /// </summary>
        public Vector3 Position { get { return camera.Position; } }

        /// <summary>
        /// The hitbox tied to the Player object, used to determine triggers and collisions.
        /// </summary>
        public BoundingSphere Hitbox { get { return hitbox; } }
        

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


        /// <summary>
        /// Construct a Player object with an hitbox but without a 3D model.
        /// </summary>
        /// <param name="hitboxRadius">Radius of the bounding sphere used as the Player's hitbox.</param>
        /// <param name="hitboxOffset">Offset of the hitbox w.r.t. the camera position.</param>
        public Player(float hitboxRadius, Vector3 hitboxOffset)
            : this(null, Vector3.Zero, Vector3.One, Quaternion.Identity, Vector3.Zero, hitboxRadius, hitboxOffset)
        {
        }

        /// <summary>
        /// Construct a Player object with a 3D model and an hitbox.
        /// </summary>
        /// <param name="playerModel">3D model of the Player entity.</param>
        /// <param name="position">Position of the Player object (the game camera) in 3D space.</param>
        /// <param name="scale">Scale of the 3D model representing the Player.</param>
        /// <param name="rotation">Initial rotation of the Player object in the game world.</param>
        /// <param name="modelOffset">Offset of the 3D model w.r.t. the camera position.</param>
        /// <param name="hitboxRadius">Radius of the bounding sphere used as the Player's hitbox.</param>
        /// <param name="hitboxOffset">Offset of the hitbox w.r.t. the camera position.</param>
        public Player(Model playerModel, Vector3 position, Vector3 scale, Quaternion rotation,
            Vector3 modelOffset, float hitboxRadius, Vector3 hitboxOffset)
        {
            jumpDuration = ((float)2 / 3 * jumpHeight) / jumpVelocity;
            jumpAcceleration = -(jumpVelocity / jumpDuration);

            sound = AudioManager.PlaySoundEffect("Minecart_Loop", true, 0.8f, 0.2f);
            sound.Pause();

            this.camera = new GameCamera(OldGoldMineGame.Graphics.GraphicsDevice.DisplayMode.AspectRatio,
                clippingPlaneNear: 0.2f)
            {
                Position = position
            };
            this.camera.SetRotation(MathHelper.ToRadians(horizontalLookAngle),
                MathHelper.ToRadians(verticalLookAngle), 0f);

            if (playerModel != null)
            {
                this.model = new GameObject3D(playerModel, position + modelOffset, scale, rotation);
                this.model.EnableDefaultLighting(specular: false);
            }

            this.hitboxOffset = hitboxOffset;
            this.hitbox = new BoundingSphere(position + hitboxOffset, hitboxRadius);
        }


        // Rotate the player's view horizontally or vertically,
        // constraining it to the maxVerticalAngle and maxHorizontalAngle values

        private void LookUpDown(float degrees)
        {
            float targetAngle = MathHelper.Clamp(verticalLookAngle + degrees,
                -maxVerticalAngle, maxVerticalAngle);

            camera.SetRotation(MathHelper.ToRadians(horizontalLookAngle),
                MathHelper.ToRadians(verticalLookAngle), 0f);
            verticalLookAngle = targetAngle;
        }

        private void LookLeftRight(float degrees)
        {
            float targetAngle = MathHelper.Clamp(horizontalLookAngle + degrees,
                -maxHorizontalAngle, maxHorizontalAngle);

            camera.SetRotation(MathHelper.ToRadians(horizontalLookAngle),
                MathHelper.ToRadians(verticalLookAngle), 0f);
            horizontalLookAngle = targetAngle;
        }


        // Fix the player's camera view back to the center 
        // (aligned with movement direction on Z axis)
        private void ResetCameraLook()
        {
            horizontalLookAngle = 0f;
            verticalLookAngle = 0f;
            camera.SetRotation(0f, 0f, 0f);

            InputManager.ResetMousePosition();
        }


        // Move the camera and the player object in the specified direction
        private void Move(float speed, Vector3 direction)
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


        /// <summary>
        /// Update the Player object's status in the current frame, handling any related input.
        /// </summary>
        /// <param name="gameTime">Time signature of the current frame.</param>
        public void Update(GameTime gameTime)
        {
            if (InputManager.FreeLookPressed)
            {
                freeLook = !freeLook;
                ResetCameraLook();
            }

            // Move the player according to the current frame inputs
            float moveSpeed = OldGoldMineGame.Application.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(moveSpeed, Vector3.Backward);

            if (InputManager.RightHold)
            {
                UpdateSideMovement(gameTime, Vector3.Right);
            }
            else if (InputManager.RightReleased)
            {
                ReverseSideMovement(Vector3.Left);
            }

            if (InputManager.LeftHold)
            {
                UpdateSideMovement(gameTime, Vector3.Left);
            }
            else if (InputManager.LeftReleased)
            {
                ReverseSideMovement(Vector3.Right);
            }

            if (InputManager.DownHold)
            {
                UpdateCrouchMovement(gameTime);
            }
            else if (InputManager.DownReleased)
            {
                ReverseCrouch();
            }

            if (InputManager.JumpPressed)
            {
                Jump();
            }

            if (freeLook)
            {
                float lookAroundSpeed = -25f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                LookUpDown(InputManager.MouseMovementY * lookAroundSpeed);
                LookLeftRight(InputManager.MouseMovementX * lookAroundSpeed);
            }

            UpdateJump(gameTime);

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


        /// <summary>
        /// Render the Player object.
        /// </summary>
        public void Draw()
        {
            if (model != null)
                model.Draw(camera);
        }



        /* Based on the type of side movement animation, the player model and camera position are
         * interpolated between the 2 keyframe positions according to the current animation timepoint */

        private void UpdateSideMovement(GameTime gameTime, Vector3 direction)
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
        private void ReverseSideMovement(Vector3 reverseDirection)
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


        private void UpdateCrouchMovement(GameTime gameTime)
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
        private void ReverseCrouch()
        {
            if (state == AnimationState.Crouch)
            {
                state = AnimationState.CrouchReverse;
                crouchTimepoint = 0f;
            }
        }


        // Start the jumping animation
        private void Jump()
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
            timeBeforeNextJump = MathHelper.Clamp
                (timeBeforeNextJump - (float)gameTime.ElapsedGameTime.TotalSeconds,
                0f, float.MaxValue);

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