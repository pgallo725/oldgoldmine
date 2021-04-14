using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;

namespace OldGoldMine.UI
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
        private Color buttonShade;

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
            set
            {
                Vector2 deltaMovement = value - this.Position;
                buttonArea.Location = value.ToPoint() - buttonArea.Size / new Point(2);
                if (buttonText != null)
                    buttonText.Position += deltaMovement;
            }
        }

        /// <summary>
        /// The pixel size of this Button element
        /// </summary>
        public Vector2 Size
        {
            get { return buttonArea.Size.ToVector2(); }
            set
            {
                Point oldPosition = buttonArea.Center;
                buttonArea.Size = value.ToPoint();
                buttonArea.Location = oldPosition - buttonArea.Size / new Point(2);
            }
        }

        /// <summary>
        /// The color shade used to filter the Button's sprite (use Color.White if you don't want color correction)
        /// </summary>
        public Color Shade { get { return buttonShade; } set { buttonShade = value; } }


        /// <summary>
        /// Create an interactive Button object with an optional text label, inside of a Rectangle area
        /// </summary>
        /// <param name="buttonArea">The Rectangle that will contain the button, defining its position and size.</param>
        /// <param name="text">SpriteText object representing the button's label.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        /// <param name="shade">Color used to filter the Button's sprite, leaving the default value is the same as passing Color.White</param>
        public Button(Rectangle buttonArea, SpriteText text, GameResources.ButtonTexturePack texturePack, Color shade = default)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = buttonArea;
            this.buttonText = text;
            this.buttonTextures = texturePack;
            this.buttonShade = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Create an interactive Button object with an optional text label, with a specified position and size
        /// </summary>
        /// <param name="position">The pixel coordinates of the center of the button.</param>
        /// <param name="size">The button size in pixels.</param>
        /// <param name="text">SpriteText object representing the button's label.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        /// <param name="shade">Color used to filter the Button's sprite, leaving the default value is the same as passing Color.White</param>
        public Button(Vector2 position, Vector2 size, SpriteText text, GameResources.ButtonTexturePack texturePack, Color shade = default)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = new Rectangle((position - size / 2).ToPoint(), size.ToPoint());
            this.buttonText = text;
            this.buttonTextures = texturePack;
            this.buttonShade = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Create an interactive Button object inside a Rectangle area, also defining the attributes of its text label
        /// </summary>
        /// <param name="buttonArea">The Rectangle that will contain the button, defining its position and size.</param>
        /// <param name="font">The SpriteFont to be used in the button's label.</param>
        /// <param name="text">Text string to be shown in the button's label.</param>
        /// <param name="textColor">Color of the label's text.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        /// <param name="shade">Color used to filter the Button's sprite, leaving the default value is the same as passing Color.White</param>
        public Button(Rectangle buttonArea, SpriteFont font, string text, Color textColor,
            GameResources.ButtonTexturePack texturePack, Color shade = default)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = buttonArea;
            this.buttonText = new SpriteText(font, text,
                buttonArea.Center.ToVector2(), SpriteText.TextAnchor.MiddleCenter);
            this.buttonTextures = texturePack;
            this.buttonShade = shade == default ? Color.White : shade;
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
        /// <param name="shade">Color used to filter the Button's sprite, leaving the default value is the same as passing Color.White</param>
        public Button(Vector2 position, Vector2 size, SpriteFont font, string text, Color textColor,
            GameResources.ButtonTexturePack texturePack, Color shade = default)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = new Rectangle((position - size/2).ToPoint(), size.ToPoint());
            this.buttonText = new SpriteText(font, text, 
                buttonArea.Center.ToVector2(), SpriteText.TextAnchor.MiddleCenter);
            this.buttonTextures = texturePack;
            this.buttonShade = shade == default ? Color.White : shade;
        }


        /// <summary>
        /// Update the Button's status in the current frame
        /// </summary>
        /// <returns>Boolean flag indicating if the Button has been clicked in the current frame.</returns>
        public bool Update()
        {
            bool interacted = false;

            if (!buttonEnabled)
            {
                buttonState = ButtonState.Disabled;
            }
            else if (buttonArea.Contains(InputManager.MousePosition))
            {
                if (buttonState != ButtonState.Pressed && InputManager.MouseSingleLeftClick)
                {
                    buttonState = ButtonState.Pressed;
                    interacted = true;
                }
                else if (!InputManager.MouseHoldLeftClick)
                    buttonState = ButtonState.Highlighted;
            }
            else buttonState = ButtonState.Normal;

            return interacted;
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTextures[(int)buttonState], buttonArea, buttonShade);

            if (buttonText != null)
                buttonText.Draw(spriteBatch);
        }
    }
}
