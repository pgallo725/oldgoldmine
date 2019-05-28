using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace oldgoldmine_game.Engine
{

    public static class InputManager
    {
        private static Point mousePosition = Point.Zero;
        private static Vector2 mouseMovement = Vector2.Zero;
        private static bool mouseLeftClickSingle = false;
        private static bool mouseLeftClickHold = false;
        private static bool pauseWasPressed = false;
        private static bool pausePressed = false;
        private static bool debugKeyWasPressed = false;
        private static bool debugKeyPressed = false;

        public static Vector2 CurrentFrameMouseMovement { get { return mouseMovement; } }
        public static Point MousePosition { get { return mousePosition; } }
        public static float MouseMovementX { get { return mouseMovement.X; } }
        public static float MouseMovementY { get { return -mouseMovement.Y; } }
        public static bool MouseSingleLeftClick { get { return mouseLeftClickSingle; } }
        public static bool MouseHoldLeftClick { get { return mouseLeftClickHold; } }
        public static bool PauseKeyPressed { get { return pausePressed; } }
        public static bool DebugKeyPressed { get { return debugKeyPressed; } }


        public static void UpdateFrameInput()
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            mouseMovement = (mouseState.Position - mousePosition).ToVector2();
            mousePosition = mouseState.Position;
            mouseLeftClickSingle = !mouseLeftClickHold && mouseState.LeftButton == ButtonState.Pressed;
            mouseLeftClickHold = mouseState.LeftButton == ButtonState.Pressed;
            pausePressed = !pauseWasPressed && (keyboardState.IsKeyDown(Keys.Escape) || gamepadState.IsButtonDown(Buttons.Start));
            pauseWasPressed = (keyboardState.IsKeyDown(Keys.Escape) || gamepadState.IsButtonDown(Buttons.Start));
            debugKeyPressed = !debugKeyWasPressed && (keyboardState.IsKeyDown(Keys.F) || gamepadState.IsButtonDown(Buttons.Back));
            debugKeyWasPressed = (keyboardState.IsKeyDown(Keys.F) || gamepadState.IsButtonDown(Buttons.Back));
        }

    }
}