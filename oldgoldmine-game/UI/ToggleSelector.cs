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
        private int value;

        public List<string> Values { get { return textValues; } }
        public int SelectedValue { get { return value; } set { this.value = value; } }



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


        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            leftButton.Draw(spriteBatch);
            rightButton.Draw(spriteBatch);

            label.Draw(spriteBatch);
        }
    }
}
