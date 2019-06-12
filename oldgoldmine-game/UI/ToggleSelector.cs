using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;

namespace oldgoldmine_game.UI
{
    class ToggleSelector : ComponentUI
    {
        private Button leftButton;
        private Button rightButton;

        private SpriteText label;

        private List<string> textValues;
        private int index;

        public List<string> Values { get { return textValues; } }
        public int SelectedValueIndex { get { return index; } set { this.index = value; } }
        public string SelectedValue { get { return Values[index]; } }



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
            this.textValues = textValues;
            if (this.textValues != null && this.textValues.Count > 0)
            {
                label = new SpriteText(font, textValues[index], position.ToVector2(), SpriteText.TextAlignment.MiddleCenter);
            }
            else
            {
                label = new SpriteText(font, "", position.ToVector2(), SpriteText.TextAlignment.MiddleCenter);
            }
        }

        public ToggleSelector(Point position, 
            Point buttonSize, Texture2D buttonNormal, Texture2D buttonHighlighted,
            SpriteFont font, List<string> textValues, int textWidth = 0)
        {

        }


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
                label.Text = textValues[index];
            }
        }

        private void RightButtonClick()
        {
            if (index < textValues.Count - 1)
            {
                index++;
                label.Text = textValues[index];
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
