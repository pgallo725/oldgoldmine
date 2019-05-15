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
        private static bool pausePressed = false;

        public static Vector2 CurrentFrameMouseMovement { get { return mouseMovement; } }
        public static Point MousePosition { get { return mousePosition; } }
        public static bool MouseSingleLeftClick { get { return mouseLeftClickSingle; } }
        public static bool MouseHoldLeftClick { get { return mouseLeftClickHold; } }
        public static bool PauseKeyPressed { get { return pausePressed; } }


        public static void UpdateFrameInput()
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            mouseMovement = (mouseState.Position - mousePosition).ToVector2();
            mousePosition = mouseState.Position;
            mouseLeftClickSingle = !mouseLeftClickHold && mouseState.LeftButton == ButtonState.Pressed;
            mouseLeftClickHold = mouseState.LeftButton == ButtonState.Pressed;
            pausePressed = !pausePressed && 
                (keyboardState.IsKeyDown(Keys.Escape) || gamepadState.IsButtonDown(Buttons.Start));
        }

    }
}