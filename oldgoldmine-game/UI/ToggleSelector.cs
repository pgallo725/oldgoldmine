using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;

namespace oldgoldmine_game.UI
{
    class ToggleSelector : IComponentUI
    {
        private Button leftButton;
        private Button rightButton;

        private SpriteText label;

        private List<string> values;
        private int index;

        /// <summary>
        /// List of string values that this element switches between
        /// </summary>
        public List<string> Values { get { return values; } }

        /// <summary>
        /// Index of the currently selected option, if set manually it also updates 
        /// the ToggleSelector status to reflect the change
        /// </summary>
        public int SelectedValueIndex
        {
            get { return index; }

            set
            {
                this.index = MathHelper.Clamp(value, 0, values.Count - 1);
                label.Text = values[index];

                if (index == 0)
                    leftButton.Enabled = false;
                else leftButton.Enabled = true;

                if (index == values.Count - 1)
                    rightButton.Enabled = false;
                else rightButton.Enabled = true;
            }
        }

        /// <summary>
        /// The string of the current option being selected
        /// </summary>
        public string SelectedValue { get { return values[index]; } }


        /// <summary>
        /// This flag determines if, the user can interact with the ToggleSelector and change the selected option,
        /// or if the buttons are greyed out and disabled (in that case, the last status is kept)
        /// </summary>
        public bool Enabled
        {
            get { return (leftButton.Enabled || rightButton.Enabled); }
            set
            {
                if (value)
                {
                    if (index == 0)
                        leftButton.Enabled = false;
                    else leftButton.Enabled = true;

                    if (index == values.Count - 1)
                        rightButton.Enabled = false;
                    else rightButton.Enabled = true;
                }
                else
                {
                    leftButton.Enabled = false;
                    rightButton.Enabled = false;
                }
            }
        }


        /// <summary>
        /// The pixel coordinates referring to the center of the ToggleSelector element
        /// </summary>
        public Vector2 Position
        {
            get { return (leftButton.Position + rightButton.Position) / 2f; }
            set
            {
                Vector2 delta = value - this.Position;
                leftButton.Position += delta;
                rightButton.Position += delta;
                label.Position += delta;
            }
        }

        /// <summary>
        /// The color used to shade the ToggleSelector left and right buttons (Color.White is the neutral value)
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
        /// Create a new ToggleSelector item inside the bounds defined by the Recangle, initializing the left and right buttons
        /// and specifying the list of values that this element will use
        /// </summary>
        /// <param name="elementArea">Rectangle that will contain the entire element, 2 buttons on the sides and text in the middle.</param>
        /// <param name="buttonSize">Size (in pixels) of the left and right buttons.</param>
        /// <param name="font">SpriteFont that will be used to draw the text of the currently selected option.</param>
        /// <param name="textValues">List of selectable items.</param>
        /// <param name="leftButtonTextures">Texture pack to use for the left button.</param>
        /// <param name="rightButtonTextures">Texture pack to use for the right button.</param>
        /// <param name="buttonShade">Color used to shade the left and right Buttons' sprites (Color.White equals leaving it at default).</param>
        public ToggleSelector(Rectangle elementArea, Vector2 buttonSize,
            SpriteFont font, List<string> textValues,
            GameResources.ButtonTexturePack leftButtonTextures, GameResources.ButtonTexturePack rightButtonTextures,
            Color buttonShade = default)
        {
            leftButton = new Button(new Vector2(elementArea.Left + buttonSize.X/2, elementArea.Height/2),
                buttonSize, null, leftButtonTextures, buttonShade);

            rightButton = new Button(new Vector2(elementArea.Right - buttonSize.X / 2, elementArea.Height / 2),
                buttonSize, null, rightButtonTextures, buttonShade);

            this.index = 0;
            this.values = textValues;

            if (this.values != null && this.values.Count > 0)
                label = new SpriteText(font, textValues[index], elementArea.Center.ToVector2(), SpriteText.TextAnchor.MiddleCenter);
            else label = new SpriteText(font, "", elementArea.Center.ToVector2(), SpriteText.TextAnchor.MiddleCenter);

            leftButton.Enabled = false;

            if (this.values == null || this.values.Count <= 1)
                rightButton.Enabled = false;
        }

        /// <summary>
        /// Create a new ToggleSelector item at the specified position, defining the space reserved to the text 
        /// in the middle and the size of the buttons
        /// </summary>
        /// <param name="position">The position of this element, in pixel coordinates, referred to its center point.</param>
        /// <param name="buttonSize">Size (in pixels) of the left and right buttons.</param>
        /// <param name="textWidth">Width (in pixels) of the central text area separating the buttons.</param>
        /// <param name="font">SpriteFont that will be used to draw the text of the currently selected option.</param>
        /// <param name="textValues">List of selectable items.</param>
        /// <param name="leftButtonTextures">Texture pack to use for the left button.</param>
        /// <param name="rightButtonTextures">Texture pack to use for the right button.</param>
        /// <param name="buttonShade">Color used to shade the left and right Buttons' sprites (Color.White equals leaving it at default).</param>
        public ToggleSelector(Vector2 position, Vector2 buttonSize,
            float textWidth, SpriteFont font, List<string> textValues,
            GameResources.ButtonTexturePack leftButtonTextures, GameResources.ButtonTexturePack rightButtonTextures,
            Color buttonShade = default)
        {
            leftButton = new Button(position - new Vector2(textWidth / 2 + buttonSize.X / 2, 0f),
                buttonSize, null, leftButtonTextures, buttonShade);

            rightButton = new Button(position + new Vector2(textWidth / 2 + buttonSize.X / 2, 0f),
                buttonSize, null, rightButtonTextures, buttonShade);

            this.index = 0;
            this.values = textValues;

            if (this.values != null && this.values.Count > 0)
                label = new SpriteText(font, textValues[index], position, SpriteText.TextAnchor.MiddleCenter);
            else label = new SpriteText(font, "", position, SpriteText.TextAnchor.MiddleCenter);

            leftButton.Enabled = false;

            if (this.values == null || this.values.Count <= 1)
                rightButton.Enabled = false;
        }


        /// <summary>
        /// Update the ToggleSelector status in the current frame
        /// </summary>
        /// <returns>Boolean flag indicating if the ToggleSelector value has changed in the current frame</returns>
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

                rightButton.Enabled = true;

                if (index == 0)
                    leftButton.Enabled = false;
            }
        }

        private void RightButtonClick()
        {
            if (index < values.Count - 1)
            {
                index++;
                label.Text = values[index];

                leftButton.Enabled = true;

                if (index == values.Count - 1)
                    rightButton.Enabled = false;
            }
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            leftButton.Draw(spriteBatch);
            rightButton.Draw(spriteBatch);

            if (label != null)
                label.Draw(spriteBatch);
        }
    }
}
