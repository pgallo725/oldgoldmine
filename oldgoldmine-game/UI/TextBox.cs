using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using oldgoldmine_game.Engine;

namespace oldgoldmine_game.UI
{
    class TextBox : IComponentUI
    {

        private enum TextboxState
        {
            Normal,
            Highlighted,
            Selected,
            Disabled
        }

        private TextboxState state = TextboxState.Normal;
        private GameResources.ButtonTexturePack textures;

        private Rectangle boxArea;
        private Rectangle textArea;

        private SpriteText content;


        /// <summary>
        /// Contains the set of keys pressed in the previous frame
        /// </summary>
        private Keys[] lastPressedKeys = new Keys[0];

        private bool caps;
        private int characterLimit;


        /// <summary>
        /// Determines if the textbox is active and interactable, or greyed out and disabled
        /// </summary>
        public bool Enabled
        {
            get { return state != TextboxState.Disabled; }
            set
            {
                if (value)
                    state = TextboxState.Normal;
                else state = TextboxState.Disabled;
            }
        }


        /// <summary>
        /// The pixel coordinates referring to the center of the TextBox
        /// </summary>
        public Vector2 Position
        {
            get { return boxArea.Center.ToVector2(); }
            set
            {
                boxArea.Location = value.ToPoint() - boxArea.Size / new Point(2);
                textArea.Location = value.ToPoint() - textArea.Size / new Point(2);
            }
        }

        /// <summary>
        /// The text content acquired by this TextBox
        /// </summary>
        public string Content { get { return content.Text; } set { content.Text = value; } }


        /// <summary>
        /// Create a TextBox inside the specified Rectangle, with the desired margin between
        /// the rectangle border and the text area
        /// </summary>
        /// <param name="area">A rectangle that will contain the TextBox element.</param>
        /// <param name="margin">Size of the separation between the element Rectangle and the area showing the text contained in it.</param>
        /// <param name="texturePack">A set of textures used to draw this element.</param>
        /// <param name="font">SpriteFont used to render the text of this element.</param>
        /// <param name="textAnchor">Specifies which corner of the text rectangle is used for alignment.</param>
        /// <param name="characterLimit">Maximum number of characters of the input string (default = unlimited)</param>
        public TextBox(Rectangle area, Vector2 margin, GameResources.ButtonTexturePack texturePack,
            SpriteFont font, SpriteText.TextAnchor textAnchor = SpriteText.TextAnchor.TopLeft, int characterLimit = int.MaxValue)
        {
            this.boxArea = area;
            this.textArea = new Rectangle(area.Location + margin.ToPoint(), area.Size - (2 * margin).ToPoint());
            this.textures = texturePack;
            this.content = new SpriteText(font, "", CalculateAnchorPoint(textArea, textAnchor).ToVector2(), textAnchor);
            this.characterLimit = characterLimit;
        }

        /// <summary>
        /// Create a TextBox at the desired position, by specifying the size of the element
        /// and the margin between the box area and the text area
        /// </summary>
        /// <param name="position">The position (in pixels) of the center of this element.</param>
        /// <param name="boxSize">Pixel size of the element.</param>
        /// <param name="margin">Size of the separation between the element Rectangle and the area showing the text contained in it.</param>
        /// <param name="texturePack">A set of textures used to draw this element.</param>
        /// <param name="font">SpriteFont used to render the text of this element.</param>
        /// <param name="textAnchor">Specifies which corner of the text rectangle is used for alignment.</param>
        /// <param name="characterLimit">Maximum number of characters of the input string (default = unlimited)</param>
        public TextBox(Vector2 position, Vector2 boxSize, Vector2 margin, GameResources.ButtonTexturePack texturePack,
            SpriteFont font, SpriteText.TextAnchor textAnchor = SpriteText.TextAnchor.TopLeft, int characterLimit = int.MaxValue)
        {
            this.boxArea = new Rectangle((position - boxSize / 2f).ToPoint(), boxSize.ToPoint());
            this.textArea = new Rectangle(boxArea.Location + margin.ToPoint(), boxArea.Size - (2 * margin).ToPoint());
            this.textures = texturePack;
            this.content = new SpriteText(font, "", CalculateAnchorPoint(textArea, textAnchor).ToVector2(), textAnchor);
            this.characterLimit = characterLimit;
        }


        private Point CalculateAnchorPoint(Rectangle area, SpriteText.TextAnchor anchor)
        {
            Point anchorPoint;

            switch (anchor)
            {
                case SpriteText.TextAnchor.TopLeft:
                    anchorPoint = new Point(area.Left, area.Top);
                    break;

                case SpriteText.TextAnchor.TopCenter:
                    anchorPoint = new Point(area.Center.X, area.Top);
                    break;

                case SpriteText.TextAnchor.TopRight:
                    anchorPoint = new Point(area.Right, area.Top);
                    break;

                case SpriteText.TextAnchor.MiddleLeft:
                    anchorPoint = new Point(area.Left, area.Center.Y);
                    break;

                case SpriteText.TextAnchor.MiddleCenter:
                    anchorPoint = area.Center;
                    break;

                case SpriteText.TextAnchor.MiddleRight:
                    anchorPoint = new Point(area.Right, area.Center.Y);
                    break;

                case SpriteText.TextAnchor.BottomLeft:
                    anchorPoint = new Point(area.Left, area.Bottom);
                    break;

                case SpriteText.TextAnchor.BottomCenter:
                    anchorPoint = new Point(area.Center.X, area.Bottom);
                    break;

                case SpriteText.TextAnchor.BottomRight:
                    anchorPoint = new Point(area.Right, area.Bottom);
                    break;

                default:
                    anchorPoint = new Point();
                    break;
            }

            return anchorPoint;
        }


        /// <summary>
        /// Update the TextBox status, based on user actions/input
        /// </summary>
        public void Update()
        {
            if (state != TextboxState.Disabled)
            {
                if (state == TextboxState.Selected)
                {
                    GetKeys();

                    if (ClickedOutside())
                        state = TextboxState.Normal;
                }
                else if (boxArea.Contains(InputManager.MousePosition))
                {
                    state = TextboxState.Highlighted;

                    if (ClickedInside())
                        state = TextboxState.Selected;
                }
                else state = TextboxState.Normal;
            }
        }

        private bool ClickedInside()
        {
            return InputManager.MouseSingleLeftClick && boxArea.Contains(InputManager.MousePosition);
        }

        private bool ClickedOutside()
        {
            return InputManager.MouseSingleLeftClick && !boxArea.Contains(InputManager.MousePosition);
        }


        private void GetKeys()
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            //check if any of the previous update's keys are no longer pressed
            foreach (Keys key in lastPressedKeys)
            {
                if (!pressedKeys.Contains(key))
                    OnKeyUp(key);
            }

            //check if the currently pressed keys were already pressed
            foreach (Keys key in pressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                    OnKeyDown(key);
            }

            //save the currently pressed keys so we can compare on the next update
            lastPressedKeys = pressedKeys;
        }


        private void OnKeyDown(Keys key)
        {
            int length = content.Text.Length;

            // Handle numbers (D0...D9, NumPad0...NumPad9 keys)
            if ((key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                int numchar = (key >= Keys.D0 && key <= Keys.D9) ?
                    (int)(key - Keys.D0) : (int)(key - Keys.NumPad0);

                if (length < characterLimit)
                    content.Text += numchar.ToString();
            }
            else if (key.ToString().Length != 1)     // Special characters have corresponding names, not symbols
            {
                if (key == Keys.Back && length > 0)    // Removes a letter from the textbox content
                {
                    content.Text = content.Text.Remove(length - 1);
                }
                else if (key == Keys.LeftShift || key == Keys.RightShift || key == Keys.CapsLock)
                {
                    caps = !caps;       // Switch caps ON or OFF when shift keys or caps lock are pressed
                }
                /*else if (key == Keys.Space && length < characterLimit)
                {
                    content.Text += " ";
                }*/
                else if (key == Keys.Enter || key == Keys.Escape)
                {
                    state = TextboxState.Normal;        // The textbox loses focus on Enter or ESC
                }
            }
            else if (length < characterLimit)      // Standard character, insert it in the string
            {
                if (!caps)
                    content.Text += key.ToString().ToLower();
                else content.Text += key.ToString();
            }
        }

        private void OnKeyUp(Keys key)
        {
            // Invert caps when one of the shift keys goes up
            if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                caps = !caps;
            }
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures[(int)state], boxArea, Color.BurlyWood);       // TODO: button color ?

            content.Draw(in spriteBatch);
        }
    }
}
