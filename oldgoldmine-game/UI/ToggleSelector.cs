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

        public List<string> Values { get { return values; } }
        public int SelectedValueIndex { get { return index; } set { this.index = value; } }
        public string SelectedValue { get { return values[index]; } }

        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public ToggleSelector(Rectangle elementArea, 
            Point buttonSize, Texture2D leftButtonNormal, Texture2D leftButtonHighlighted,
            Texture2D rightButtonNormal, Texture2D rightButtonHighlighted,
            SpriteFont font, List<string> textValues, int textWidth = 0)
        {

        }

        public ToggleSelector(Rectangle elementArea, 
            Point buttonSize, Texture2D buttonNormal, Texture2D buttonHighlighted,
            SpriteFont font, List<string> textValues, int textWidth = 0)
        {

        }

        public ToggleSelector(Point position, 
            Point buttonSize, Texture2D leftButtonNormal, Texture2D leftButtonHighlighted,
            Texture2D rightButtonNormal, Texture2D rightButtonHighlighted,
            SpriteFont font, List<string> textValues, int textWidth = 0)
        {
            leftButton = new Button(position - new Point(textWidth / 2 + buttonSize.X / 2, 0),
                buttonSize, null, leftButtonNormal, leftButtonHighlighted);

            rightButton = new Button(position + new Point(textWidth / 2 + buttonSize.X / 2, 0),
                buttonSize, null, rightButtonNormal, rightButtonHighlighted);

            this.index = 0;
            this.values = textValues;

            if (this.values != null && this.values.Count > 0)
            {
                label = new SpriteText(font, textValues[index], position.ToVector2(), SpriteText.TextAnchor.MiddleCenter);
            }
            else
            {
                label = new SpriteText(font, "", position.ToVector2(), SpriteText.TextAnchor.MiddleCenter);
            }

            leftButton.Enabled = false;

            if (this.values == null || this.values.Count <= 1)
                rightButton.Enabled = false;
        }

        public ToggleSelector(Point position, 
            Point buttonSize, Texture2D buttonNormal, Texture2D buttonHighlighted,
            SpriteFont font, List<string> textValues, int textWidth = 0)
        {

        }


        /// <summary>
        /// Update the ToggleSelector status in the current frame
        /// </summary>
        public void Update()
        {
            leftButton.Update();
            rightButton.Update();

            if (leftButton.IsClicked())
                LeftButtonClick();
            else if (rightButton.IsClicked())
                RightButtonClick();
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
