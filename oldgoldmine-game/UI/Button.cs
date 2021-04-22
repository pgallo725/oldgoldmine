using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.Engine;

namespace OldGoldMine.UI
{
    public class Button : IComponentUI
    {
        public struct SpritePack
        {
            public Texture2D normal;
            public Texture2D highlighted;
            public Texture2D pressed;
            public Texture2D disabled;

            /// <summary>
            /// Create a new texture pack to define the look of a Button.
            /// </summary>
            /// <param name="normal">Texture of the button used when in the Normal state.</param>
            /// <param name="highlighted">Texture used to replace the normal look of the button when highlighted.</param>
            /// <param name="pressed">Texture used to replace the normal look when the button is pressed.</param>
            /// <param name="disabled">Texture used to replace the normal look when the button is disabled.</param>
            public SpritePack(Texture2D normal, Texture2D highlighted = null, Texture2D pressed = null, Texture2D disabled = null)
            {
                this.normal = normal;
                this.highlighted = highlighted ?? normal;
                this.pressed = pressed ?? normal;
                this.disabled = disabled ?? normal;
            }

            // 0: Normal, 1: Highlighted, 2: Pressed, 3: Disabled
            public Texture2D this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return this.normal;
                        case 1: return this.highlighted;
                        case 2: return this.pressed;
                        case 3: return this.disabled;
                        default: throw new System.IndexOutOfRangeException();
                    }
                }

                set
                {
                    switch (i)
                    {
                        case 0: this.normal = value; break;
                        case 1: this.highlighted = value; break;
                        case 2: this.pressed = value; break;
                        case 3: this.disabled = value; break;
                        default: throw new System.IndexOutOfRangeException();
                    }
                }
            }
        }

        private enum ButtonState
        {
            Normal,
            Highlighted,
            Pressed,
            Disabled
        }

        private ButtonState buttonState;
        private SpritePack buttonTextures;
        private Rectangle buttonArea;


        /// <summary>
        /// Determines if the button is active and interactable, or greyed out and disabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The pixel coordinates referring to the center of the Button.
        /// </summary>
        public Point Position
        {
            get { return buttonArea.Center; }
            set
            {
                if (buttonText != null)
                    buttonText.Position += value - this.Position;       // Maintain text anchoring offset
                buttonArea.Location = value - Size / new Point(2);
            }
        }

        /// <summary>
        /// The pixel size of this Button element.
        /// </summary>
        public Point Size
        {
            get { return buttonArea.Size; }
            set
            {
                buttonArea.Location = buttonArea.Center - value / new Point(2);
                buttonArea.Size = value;
            }
        }

        /// <summary>
        /// The content of the Button's label.
        /// </summary>
        public string Text { get { return buttonText.Text; } set { buttonText.Text = value; } }
        private readonly SpriteText buttonText;

        /// <summary>
        /// The color of the text label inside this Button.
        /// </summary>
        public Color TextColor { get { return buttonText.Color; } set { buttonText.Color = value; } }

        /// <summary>
        /// The color shade used to filter the Button's sprite (Color.White preserves original color).
        /// </summary>
        public Color Shade { get { return buttonShade; } set { buttonShade = value; } }
        private Color buttonShade;


        /// <summary>
        /// Create an interactive Button inside a Rectangle area, and define the attributes of its text label.
        /// </summary>
        /// <param name="area">The Rectangle that will contain the Button, defining its position and size.</param>
        /// <param name="font">The SpriteFont to be used in the Button's label.</param>
        /// <param name="text">Text string to be shown in the Button's label.</param>
        /// <param name="textColor">Color of the label's text.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        /// <param name="shade">Color used to filter the Button's sprite, Color.White (default) preserves original colors.</param>
        public Button(Rectangle area, SpriteFont font, string text, Color textColor, SpritePack texturePack, Color shade = default)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = area;
            this.buttonText = new SpriteText(font, text, textColor, buttonArea.Center);
            this.buttonTextures = texturePack;
            this.buttonShade = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Create an interactive Button with no text inside a Rectangle area.
        /// </summary>
        /// <param name="area">The Rectangle that will contain the Button, defining its position and size.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        /// <param name="shade">Color used to filter the Button's sprite, Color.White (default) preserves original colors.</param>
        public Button(Rectangle area, SpritePack texturePack, Color shade = default)
        {
            this.buttonState = ButtonState.Normal;
            this.buttonArea = area;
            this.buttonText = null;
            this.buttonTextures = texturePack;
            this.buttonShade = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Create an interactive Button with a specified position and size, and define the attributes of its text label.
        /// </summary>
        /// <param name="position">The pixel coordinates of the center of the Button.</param>
        /// <param name="size">The size of the Button in pixels.</param>
        /// <param name="font">The SpriteFont to be used in the Button's label.</param>
        /// <param name="text">Text string to be shown in the Button's label.</param>
        /// <param name="textColor">Color of the label's text.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        /// <param name="shade">Color used to filter the Button's sprite, Color.White (default) preserves original colors.</param>
        public Button(Point position, Point size, SpriteFont font, string text, Color textColor,
            SpritePack texturePack, Color shade = default)
            : this(new Rectangle(position - size / new Point(2), size), font, text, textColor, texturePack, shade)
        {
        }

        /// <summary>
        /// Create an interactive Button with no text at a specified position and size.
        /// </summary>
        /// <param name="position">The pixel coordinates of the center of the Button.</param>
        /// <param name="size">The size of the Button in pixels.</param>
        /// <param name="texturePack">Texture pack with the different possible looks of the Button.</param>
        /// <param name="shade">Color used to filter the Button's sprite, Color.White (default) preserves original colors.</param>
        public Button(Point position, Point size, SpritePack texturePack, Color shade = default)
            : this(new Rectangle(position - size / new Point(2), size), texturePack, shade)
        {
        }


        /// <summary>
        /// Update the Button's status in the current frame.
        /// </summary>
        /// <returns>Boolean flag indicating if the Button has been clicked in the current frame.</returns>
        public bool Update()
        {
            bool interacted = false;

            if (!Enabled)
            {
                buttonState = ButtonState.Disabled;
            }
            else if (buttonArea.Contains(InputManager.MousePosition))
            {
                if (buttonState != ButtonState.Pressed && InputManager.MouseLeftButtonClick)
                {
                    buttonState = ButtonState.Pressed;
                    interacted = true;
                }
                else if (!InputManager.MouseLeftButtonHold)
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
