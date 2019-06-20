using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;

namespace oldgoldmine_game.UI
{
    class Button : IComponentUI
    {
        private enum ButtonState
        {
            Normal,
            Highlighted,
            Pressed,
            Disabled
        }

        private bool buttonEnabled = true;
        private ButtonState buttonState;

        private Rectangle buttonArea;
        private SpriteText buttonText;

        // 0: Normal, 1: Highlighted, 2: Pressed, 3: Disabled
        private GameResources.ButtonTexturePack buttonTextures;


        /// <summary>
        /// Determines if the button is active and interactable, or greyed out and disabled
        /// </summary>
        public bool Enabled { get { return buttonEnabled; } set { buttonEnabled = value; } }

        /// <summary>
        /// The pixel coordinates referring to the center of the Button
        /// </summary>
        public Vector2 Position
        {
            get { return buttonArea.Center.ToVector2(); }
            set { buttonArea.Location = value.ToPoint() - buttonArea.Size / new Point(2); }
        }

        // TODO: size ?


        /// <summary>
        /// Create an interactive Button object with an optional text label, inside of a Rectangle area
        /// </summary>
        /// <param name="buttonArea">The Rectangle that will contain the button, defining its position and size.</param>
        /// <param name="text">SpriteText object representing the button's label.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        public Button(Rectangle buttonArea, SpriteText text, GameResources.ButtonTexturePack texturePack)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = buttonArea;
            this.buttonText = text;
            this.buttonTextures = texturePack;
        }

        /// <summary>
        /// Create an interactive Button object with an optional text label, with a specified position and size
        /// </summary>
        /// <param name="position">The pixel coordinates of the center of the button.</param>
        /// <param name="size">The button size in pixels.</param>
        /// <param name="text">SpriteText object representing the button's label.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        public Button(Vector2 position, Vector2 size, SpriteText text, GameResources.ButtonTexturePack texturePack)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = new Rectangle((position - size / 2).ToPoint(), size.ToPoint());
            this.buttonText = text;
            this.buttonTextures = texturePack;
        }

        /// <summary>
        /// Create an interactive Button object inside a Rectangle area, also defining the attributes of its text label
        /// </summary>
        /// <param name="buttonArea">The Rectangle that will contain the button, defining its position and size.</param>
        /// <param name="font">The SpriteFont to be used in the button's label.</param>
        /// <param name="text">Text string to be shown in the button's label.</param>
        /// <param name="textColor">Color of the label's text.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        public Button(Rectangle buttonArea, SpriteFont font, string text, Color textColor,
            GameResources.ButtonTexturePack texturePack)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = buttonArea;
            this.buttonText = new SpriteText(font, text,
                buttonArea.Center.ToVector2(), SpriteText.TextAnchor.MiddleCenter);
            this.buttonTextures = texturePack;
        }

        /// <summary>
        /// Create an interactive Button object with a specified position and size, also defining the attributes of its text label
        /// </summary>
        /// <param name="position">The pixel coordinates of the center of the button.</param>
        /// <param name="size">The button size in pixels.</param>
        /// <param name="font">The SpriteFont to be used in the button's label.</param>
        /// <param name="text">Text string to be shown in the button's label.</param>
        /// <param name="textColor">Color of the label's text.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        public Button(Vector2 position, Vector2 size, SpriteFont font, string text, Color textColor,
            GameResources.ButtonTexturePack texturePack)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = new Rectangle((position - size/2).ToPoint(), size.ToPoint());
            this.buttonText = new SpriteText(font, text, 
                buttonArea.Center.ToVector2(), SpriteText.TextAnchor.MiddleCenter);
            this.buttonTextures = texturePack;
        }


        /// <summary>
        /// Update the Button's status in the current frame
        /// </summary>
        public void Update()
        {
            if (!buttonEnabled)
            {
                buttonState = ButtonState.Disabled;
            }
            else if (buttonArea.Contains(InputManager.MousePosition))
            {
                if (buttonState != ButtonState.Pressed && IsClicked())
                    buttonState = ButtonState.Pressed;
                else if (!InputManager.MouseHoldLeftClick)
                    buttonState = ButtonState.Highlighted;
            }
            else buttonState = ButtonState.Normal;
        }


        /// <summary>
        /// Check if the button has been pressed during the current frame
        /// </summary>
        public bool IsClicked()
        {
            return buttonEnabled &&
                InputManager.MouseSingleLeftClick && buttonArea.Contains(InputManager.MousePosition);
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTextures[(int)buttonState],
                buttonArea, Color.BurlyWood);                   // TODO: button color ?

            if (buttonText != null)
                buttonText.Draw(spriteBatch);
        }
    }
}
