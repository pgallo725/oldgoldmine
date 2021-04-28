using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Static class that acts as a single input processing unit, from which
    /// other components can access their required information every frame.
    /// </summary>
    public static class InputManager
    {
        public static Point MouseMovement { get; private set; } = Point.Zero;
        public static Point MousePosition { get; private set; } = Point.Zero;
        public static float MouseMovementX { get { return MouseMovement.X; } }
        public static float MouseMovementY { get { return -MouseMovement.Y; } }

        public static bool MouseLeftButtonClick { get; private set; }
        public static bool MouseLeftButtonHold { get; private set; }
        public static bool MouseRightButtonClick { get; private set; }
        public static bool MouseRightButtonHold { get; private set; }
        public static bool MouseMiddleButtonClick { get; private set; }
        public static bool MouseMiddleButtonHold { get; private set; }

        public static bool CapsActive { get; private set; }
        public static HashSet<Keys> PressedKeys { get { return keysPresssed; } }
        
        public static bool PausePressed { get { return keysPresssed.Contains(Keys.Escape) || buttonsPressed.Contains(Buttons.Start); } }
        public static bool FreeLookPressed { get { return keysPresssed.Contains(Keys.F) || buttonsPressed.Contains(Buttons.Back); } }
        public static bool DebugPressed { get { return keysPresssed.Contains(Keys.G); } }
        public static bool EnterPressed { get { return keysPresssed.Contains(Keys.Enter) || buttonsPressed.Contains(Buttons.A); } }
        public static bool BackPressed { get { return keysPresssed.Contains(Keys.Escape) || buttonsPressed.Contains(Buttons.B); } }

        public static bool LeftHold 
        { 
            get
            { 
                return keysDown.Contains(Keys.A) || keysDown.Contains(Keys.Left) ||
                    buttonsDown.Contains(Buttons.DPadLeft) || buttonsDown.Contains(Buttons.LeftThumbstickLeft);
            } 
        }
        public static bool RightHold
        {
            get
            {
                return keysDown.Contains(Keys.D) || keysDown.Contains(Keys.Right) ||
                    buttonsDown.Contains(Buttons.DPadRight) || buttonsDown.Contains(Buttons.LeftThumbstickRight);
            }
        }
        public static bool DownHold
        {
            get
            {
                return keysDown.Contains(Keys.S) || keysDown.Contains(Keys.Down) || keysDown.Contains(Keys.LeftControl) ||
                    buttonsDown.Contains(Buttons.DPadDown) || buttonsDown.Contains(Buttons.LeftThumbstickDown);
            }
        }
        public static bool LeftReleased
        {
            get
            {
                return keysReleased.Contains(Keys.A) || keysReleased.Contains(Keys.Left) ||
                    buttonsReleased.Contains(Buttons.DPadLeft) || buttonsReleased.Contains(Buttons.LeftThumbstickLeft);
            }
        }
        public static bool RightReleased
        {
            get
            {
                return keysReleased.Contains(Keys.D) || keysReleased.Contains(Keys.Right) ||
                    buttonsReleased.Contains(Buttons.DPadRight) || buttonsReleased.Contains(Buttons.LeftThumbstickRight);
            }
        }
        public static bool DownReleased
        {
            get
            {
                return keysReleased.Contains(Keys.S) || keysReleased.Contains(Keys.Down) || keysReleased.Contains(Keys.LeftControl) ||
                    buttonsReleased.Contains(Buttons.DPadDown) || buttonsReleased.Contains(Buttons.LeftThumbstickDown);
            }
        }
        public static bool JumpPressed 
        { 
            get
            { 
                return keysPresssed.Contains(Keys.Space) || keysPresssed.Contains(Keys.W) || keysPresssed.Contains(Keys.Up) ||
                    buttonsPressed.Contains(Buttons.A) || buttonsPressed.Contains(Buttons.DPadUp) || buttonsPressed.Contains(Buttons.LeftThumbstickUp); 
            } 
        }


        private readonly static HashSet<Keys> previousKeys = new HashSet<Keys>();
        private readonly static HashSet<Keys> keysPresssed = new HashSet<Keys>();
        private readonly static HashSet<Keys> keysDown = new HashSet<Keys>();
        private readonly static HashSet<Keys> keysReleased = new HashSet<Keys>();

        private readonly static HashSet<Buttons> previousButtons = new HashSet<Buttons>();
        private readonly static HashSet<Buttons> buttonsPressed = new HashSet<Buttons>();
        private readonly static HashSet<Buttons> buttonsDown = new HashSet<Buttons>();
        private readonly static HashSet<Buttons> buttonsReleased = new HashSet<Buttons>();


        /// <summary>
        /// Process the input for the current frame (mouse, keyboard and gamepad).
        /// </summary>
        public static void UpdateFrameInput()
        {
            // Retrieve the state from all supported input peripherals (mouse, keyboard, gamepad)
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            // MOUSE
            // Check user input (both position and clicks)
            MouseMovement = mouseState.Position - MousePosition;
            MousePosition = mouseState.Position;
            MouseLeftButtonClick = !MouseLeftButtonHold && mouseState.LeftButton == ButtonState.Pressed;
            MouseLeftButtonHold = mouseState.LeftButton == ButtonState.Pressed;
            MouseRightButtonClick = !MouseRightButtonHold && mouseState.RightButton == ButtonState.Pressed;
            MouseRightButtonHold = mouseState.RightButton == ButtonState.Pressed;
            MouseMiddleButtonClick = !MouseMiddleButtonHold && mouseState.MiddleButton == ButtonState.Pressed;
            MouseMiddleButtonHold = mouseState.MiddleButton == ButtonState.Pressed;

            // KEYBOARD
            // Determine which keys have been pressed, released or held down in the current frame
            keysDown.Clear();
            keysDown.UnionWith(keyboardState.GetPressedKeys());

            keysPresssed.Clear();
            keysPresssed.UnionWith(keysDown);
            keysPresssed.ExceptWith(previousKeys);

            keysReleased.Clear();
            keysReleased.UnionWith(previousKeys);
            keysReleased.ExceptWith(keysDown);

            // Update the status of the Caps based on current keypresses
            if (keysPresssed.Contains(Keys.CapsLock) || keysPresssed.Contains(Keys.LeftShift) || keysPresssed.Contains(Keys.RightShift))
            {
                CapsActive = !CapsActive;       // Switch caps ON or OFF when shift keys or caps lock are pressed
            }
            if (keysReleased.Contains(Keys.LeftShift) || keysReleased.Contains(Keys.RightShift))
            {
                CapsActive = !CapsActive;       // Invert caps when one of the shift keys goes up
            }

            // Save the currently pressed keys for the next update
            previousKeys.Clear();
            previousKeys.UnionWith(keysDown);

            // GAMEPAD
            // Determine which buttons have been pressed, released or held down in the current frame
            buttonsDown.Clear();
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                if (gamepadState.IsButtonDown(button))
                    buttonsDown.Add(button);
            }

            buttonsPressed.Clear();
            buttonsPressed.UnionWith(buttonsDown);
            buttonsPressed.ExceptWith(previousButtons);

            buttonsReleased.Clear();
            buttonsReleased.UnionWith(previousButtons);
            buttonsReleased.ExceptWith(buttonsDown);

            // Save the currently pressed buttons for the next update
            previousButtons.Clear();
            previousButtons.UnionWith(buttonsDown);
        }


        /// <summary>
        /// Reset the mouse cursor position at the center of the screen.
        /// </summary>
        public static void ResetMousePosition()
        {
            Vector2 screenCenter = new Vector2(OldGoldMineGame.graphics.GraphicsDevice.Viewport.Width,
                OldGoldMineGame.graphics.GraphicsDevice.Viewport.Height) / 2;

            MousePosition = screenCenter.ToPoint();
            Mouse.SetPosition(MousePosition.X, MousePosition.Y);
        }
    }
}