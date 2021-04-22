using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.UI
{
    public class Selector : IComponentUI
    {
        /* UI ELEMENTS */
        private readonly Button leftButton;
        private readonly Button rightButton;
        private readonly SpriteText label;

        /// <summary>
        /// Determines if the user can interact with the Selector and change the selected option,
        /// or if the buttons are greyed out and disabled (frozen at the last selected value).
        /// </summary>
        public bool Enabled
        {
            get { return leftButton.Enabled || rightButton.Enabled; }
            set
            {
                leftButton.Enabled = value && index > 0;
                rightButton.Enabled = value && index < (values.Count - 1);
            }
        }

        /// <summary>
        /// The pixel coordinates referring to the center of the Selector.
        /// </summary>
        public Point Position
        {
            get { return (leftButton.Position + rightButton.Position) / new Point(2); }
            set
            {
                Point delta = value - this.Position;
                leftButton.Position += delta;
                rightButton.Position += delta;
                label.Position += delta;
            }
        }

        /// <summary>
        /// The total size of the Selector, in pixels.
        /// </summary>
        public Point Size
        {
            get { return new Point(rightButton.Position.X - leftButton.Position.X + leftButton.Size.X, leftButton.Size.Y); }
            set 
            {
                leftButton.Position = label.Position - new Point(value.X / 2, 0);
                rightButton.Position = label.Position + new Point(value.X / 2, 0);
            }
        }

        /// <summary>
        /// The color used to shade the Selector left and right buttons (Color.White preserves original colors)
        /// </summary>
        public Color Shade
        {
            get { return leftButton.Shade; }
            set
            {
                leftButton.Shade = value;
                rightButton.Shade = value;
            }
        }

        /// <summary>
        /// The color of the text showed between the two buttons of the Selector.
        /// </summary>
        public Color TextColor
        {
            get { return label.Color; }
            set { label.Color = value; }
        }


        /* DATA */
        private readonly List<string> values;
        private int index;

        /// <summary>
        /// List of values that this element switches between.
        /// </summary>
        public List<string> Values { get { return values; } }

        /// <summary>
        /// Index of the currently selected option, if set manually it also 
        /// updates the Selector status to reflect the change.
        /// </summary>
        public int SelectedValueIndex
        {
            get { return index; }

            set
            {
                this.index = MathHelper.Clamp(value, 0, values.Count - 1);
                label.Text = values[index];

                leftButton.Enabled = this.Enabled && index > 0;
                rightButton.Enabled = this.Enabled && index < values.Count - 1;
            }
        }

        /// <summary>
        /// The value of the current option being selected.
        /// </summary>
        public string SelectedValue { get { return values[index]; } }


        /// <summary>
        /// Create a new Selector item inside the bounds defined by the Rectangle and
        /// specifying the list of possible values displayed by this element.
        /// </summary>
        /// <param name="area">Rectangle that will contain the entire element, 2 buttons on the sides and text in the middle.</param>
        /// <param name="buttonSize">Size (in pixels) of the left and right buttons.</param>
        /// <param name="textFont">SpriteFont that will be used to draw the text of the currently selected option.</param>
        /// <param name="textValues">List of selectable items.</param>
        /// <param name="textColor">Color of the text displaying the currently selected option.</param>
        /// <param name="buttonTexturesLeft">Texture pack to use for the left button.</param>
        /// <param name="buttonTexturesRight">Texture pack to use for the right button.</param>
        /// <param name="buttonShade">Color used to shade the left and right buttons' sprites (Color.White equals leaving it at default).</param>
        public Selector(Rectangle area, Point buttonSize, SpriteFont textFont, List<string> textValues, Color textColor,
            Button.SpritePack buttonTexturesLeft, Button.SpritePack buttonTexturesRight, Color buttonShade = default)
        {
            leftButton = new Button(area.Center - new Point(area.Width / 2 - buttonSize.X / 2, 0), 
                buttonSize, buttonTexturesLeft, buttonShade);
            rightButton = new Button(area.Center + new Point(area.Width / 2 - buttonSize.X / 2, 0),
                buttonSize, buttonTexturesRight, buttonShade);

            this.index = 0;
            this.values = textValues;

            if (this.values != null && this.values.Count > 0)
                label = new SpriteText(textFont, textValues[index], textColor, area.Center, SpriteText.TextAnchor.MiddleCenter);
            else label = new SpriteText(textFont, string.Empty, textColor, area.Center, SpriteText.TextAnchor.MiddleCenter);

            leftButton.Enabled = false;
            rightButton.Enabled = (this.values != null && this.values.Count > 1);
        }

        /// <summary>
        /// Create a new Selector item at the specified position, defining the space reserved to the text 
        /// in the middle and the size of the buttons.
        /// </summary>
        /// <param name="position">The position of this element, in pixel coordinates, referred to its center point.</param>
        /// <param name="buttonSize">Size (in pixels) of the left and right buttons.</param>
        /// <param name="textWidth">Width (in pixels) of the central text area separating the buttons.</param>
        /// <param name="textFont">SpriteFont that will be used to draw the text of the currently selected option.</param>
        /// <param name="textValues">List of selectable items.</param>
        /// <param name="textColor">Color of the text displaying the currently selected option.</param>
        /// <param name="buttonTexturesLeft">Texture pack to use for the left button.</param>
        /// <param name="buttonTexturesRight">Texture pack to use for the right button.</param>
        /// <param name="buttonShade">Color used to shade the left and right buttons' sprites (Color.White equals leaving it at default).</param>
        public Selector(Point position, Point buttonSize, int textWidth,
            SpriteFont textFont, List<string> textValues, Color textColor,
            Button.SpritePack buttonTexturesLeft, Button.SpritePack buttonTexturesRight, Color buttonShade = default)
            : this(new Rectangle(position.X - textWidth / 2 - buttonSize.X, position.Y - buttonSize.Y / 2,
                2 * buttonSize.X + textWidth, buttonSize.Y),
                  buttonSize, textFont, textValues, textColor, buttonTexturesLeft, buttonTexturesRight, buttonShade)
        {
        }


        /// <summary>
        /// Update the Selector's status in the current frame.
        /// </summary>
        /// <returns>Boolean flag indicating if the Selector value has changed in the current frame.</returns>
        public bool Update()
        {
            bool interacted = false;

            if (leftButton.Update())
            {
                LeftButtonClick();
                interacted = true;
            }
            if (rightButton.Update())
            {
                RightButtonClick();
                interacted = true;
            }
            
            return interacted;
        }


        private void LeftButtonClick()
        {
            if (index > 0)
            {
                index--;
                label.Text = values[index];

                leftButton.Enabled = index > 0;
                rightButton.Enabled = true;
            }
        }

        private void RightButtonClick()
        {
            if (index < values.Count - 1)
            {
                index++;
                label.Text = values[index];

                leftButton.Enabled = true;
                rightButton.Enabled = (index < values.Count - 1);
            }
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            leftButton.Draw(spriteBatch);
            rightButton.Draw(spriteBatch);
            label.Draw(spriteBatch);
        }
    }
}
