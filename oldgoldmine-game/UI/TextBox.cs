using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OldGoldMine.Engine;

namespace OldGoldMine.UI
{
    public class TextBox : IComponentUI
    {
        private enum TextboxState
        {
            Normal,
            Highlighted,
            Selected,
            Disabled
        }

        private TextboxState state = TextboxState.Normal;
        private Button.SpritePack textures;
        
        /* UI ELEMENTS */
        private Rectangle boxArea;
        private Rectangle textArea;
        private readonly SpriteText boxContent;

        /// <summary>
        /// Determines if the TextBox is active and interactable, or greyed out and disabled.
        /// </summary>
        public bool Enabled
        {
            get { return state != TextboxState.Disabled; }
            set { state = value ? TextboxState.Normal : TextboxState.Disabled; }
        }

        /// <summary>
        /// The pixel coordinates referring to the center of the TextBox.
        /// </summary>
        public Point Position
        {
            get { return boxArea.Center; }
            set
            {
                boxArea.Location = value - boxArea.Size / new Point(2);
                textArea.Location = value - textArea.Size / new Point(2);
                boxContent.Position = CalculateAnchorPoint(textArea, boxContent.Anchor);
            }
        }


        /// <summary>
        /// The pixel size of this TextBox element and its content.
        /// </summary>
        public Point Size
        {
            get { return boxArea.Size; }
            set
            {
                // Compute the ratio as a Vector2 to maintain floating point precision (not rounding to integers)
                Vector2 textAreaRatio = textArea.Size.ToVector2() / boxArea.Size.ToVector2();
                Point oldPosition = boxArea.Center;
                boxArea.Size = value;
                boxArea.Location = oldPosition - boxArea.Size / new Point(2);
                textArea.Size = (value.ToVector2() * textAreaRatio).ToPoint();
                textArea.Location = oldPosition - textArea.Size / new Point(2);
                boxContent.Position = CalculateAnchorPoint(textArea, boxContent.Anchor);
            }
        }

        /// <summary>
        /// The color shade used to filter the TextBox's sprite (Color.White preserves original colors).
        /// </summary>
        public Color Shade { get { return boxShade; } set { boxShade = value; } }
        private Color boxShade;

        /// <summary>
        /// The color of the text content displayed in the TextBox.
        /// </summary>
        public Color TextColor { get { return boxContent.Color; } set { boxContent.Color = value; } }


        /// <summary>
        /// The current text contained by this TextBox.
        /// </summary>
        public string Content { get { return boxContent.Text; } set { boxContent.Text = value; } }

        /// <summary>
        /// The maximum number of characters containable by this TextBox.
        /// </summary>
        public int CharacterLimit { get; set; }


        /// <summary>
        /// Create a TextBox inside the specified Rectangle, with the desired margin between the border and the text.
        /// </summary>
        /// <param name="area">A Rectangle that will contain the TextBox element.</param>
        /// <param name="margin">Size (in pixels) of the separation between the element's border and the text.</param>
        /// <param name="texturePack">A set of textures used to draw this element.</param>
        /// <param name="textFont">SpriteFont used to render the text of this element.</param>
        /// <param name="textColor">Color of the TextBox's content.</param>
        /// <param name="textAnchor">Specifies which corner of the text rectangle is used for alignment.</param>
        /// <param name="charLimit">Maximum number of characters of the input string (default = unlimited).</param>
        /// <param name="shade">Color used to shade the element's sprite (Color.White preserves original colors).</param>
        public TextBox(Rectangle area, Point margin, Button.SpritePack texturePack, SpriteFont textFont, Color textColor,
            SpriteText.TextAnchor textAnchor = SpriteText.TextAnchor.TopLeft, int charLimit = int.MaxValue, Color shade = default)
        {
            this.boxArea = area;
            this.textArea = new Rectangle(area.Location + margin, area.Size - margin * new Point(2));
            this.textures = texturePack;
            this.boxContent = new SpriteText(textFont, string.Empty, textColor, CalculateAnchorPoint(textArea, textAnchor), textAnchor);
            this.CharacterLimit = charLimit;
            this.boxShade = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Create a TextBox at the desired position, by specifying its size and the margin between the border and the text.
        /// </summary>
        /// <param name="position">The position (in pixels) of the center of this element.</param>
        /// <param name="boxSize">The size of the TextBox in pixels.</param>
        /// <param name="margin">Size (in pixels) of the separation between the element's border and the text.</param>
        /// <param name="texturePack">A set of textures used to draw this element.</param>
        /// <param name="textFont">SpriteFont used to render the text of this element.</param>
        /// <param name="textColor">Color of the TextBox's content.</param>
        /// <param name="textAnchor">Specifies which corner of the text rectangle is used for alignment.</param>
        /// <param name="charLimit">Maximum number of characters of the input string (default = unlimited)</param>
        /// <param name="shade">Color used to shade the element's sprite (the default value is the same as Color.White)</param>
        public TextBox(Point position, Point boxSize, Point margin, Button.SpritePack texturePack,
            SpriteFont textFont, Color textColor, SpriteText.TextAnchor textAnchor = SpriteText.TextAnchor.TopLeft,
            int charLimit = int.MaxValue, Color shade = default)
            : this(new Rectangle(position - boxSize / new Point(2), boxSize), margin,
                  texturePack, textFont, textColor, textAnchor, charLimit, shade)
        {
        }


        private Point CalculateAnchorPoint(Rectangle area, SpriteText.TextAnchor anchor)
        {
            return anchor switch
            {
                SpriteText.TextAnchor.TopLeft => new Point(area.Left, area.Top),
                SpriteText.TextAnchor.TopCenter => new Point(area.Center.X, area.Top),
                SpriteText.TextAnchor.TopRight => new Point(area.Right, area.Top),
                SpriteText.TextAnchor.MiddleLeft => new Point(area.Left, area.Center.Y),
                SpriteText.TextAnchor.MiddleCenter => area.Center,
                SpriteText.TextAnchor.MiddleRight => new Point(area.Right, area.Center.Y),
                SpriteText.TextAnchor.BottomLeft => new Point(area.Left, area.Bottom),
                SpriteText.TextAnchor.BottomCenter => new Point(area.Center.X, area.Bottom),
                SpriteText.TextAnchor.BottomRight => new Point(area.Right, area.Bottom),
                _ => new Point(),
            };
        }


        /// <summary>
        /// Update the TextBox status, based on user actions/input in the current frame.
        /// </summary>
        /// <returns>Boolean flag indicating if the content of the TextBox has changed in the current frame.</returns>
        public bool Update()
        {
            bool interacted = false;

            if (state != TextboxState.Disabled)
            {
                if (state == TextboxState.Selected)
                {
                    interacted = HandleKeys();

                    // If the user clicked outside of the TextBox area
                    if (InputManager.MouseLeftButtonClick && !boxArea.Contains(InputManager.MousePosition))
                        state = TextboxState.Normal;
                }
                else if (boxArea.Contains(InputManager.MousePosition))
                {
                    state = TextboxState.Highlighted;

                    // If the user clicked inside the TextBox area
                    if (InputManager.MouseLeftButtonClick && boxArea.Contains(InputManager.MousePosition))
                        state = TextboxState.Selected;
                }
                else state = TextboxState.Normal;
            }

            return interacted;
        }

        
        private bool HandleKeys()
        {
            foreach (Keys key in InputManager.PressedKeys)
            {
                int length = boxContent.Text.Length;

                // Handle numbers (D0...D9, NumPad0...NumPad9 keys)
                if ((key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9))
                {
                    if (length < CharacterLimit)
                    {
                        int numchar = (key >= Keys.D0 && key <= Keys.D9) ?
                            (key - Keys.D0) : (key - Keys.NumPad0);

                        boxContent.Text += numchar.ToString();
                    }
                }
                else if (key.ToString().Length != 1)     // Special characters have names, not symbols
                {
                    if (key == Keys.Back && length > 0)    // Removes a letter from the textbox content
                    {
                        boxContent.Text = boxContent.Text.Remove(length - 1);
                    }
                    else if (key == Keys.Space && length < CharacterLimit)
                    {
                        boxContent.Text += " ";
                    }
                    else if (key == Keys.Enter || key == Keys.Escape)
                    {
                        state = TextboxState.Normal;        // The textbox loses focus on Enter or ESC
                    }
                }
                else if (length < CharacterLimit)      // Standard character, insert it in the string
                {
                    if (!InputManager.CapsActive)
                        boxContent.Text += key.ToString().ToLower();
                    else boxContent.Text += key.ToString();
                }
            }

            return InputManager.PressedKeys.Count > 0;
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures[(int)state], boxArea, boxShade);

            boxContent.Draw(in spriteBatch);
        }
    }
}
