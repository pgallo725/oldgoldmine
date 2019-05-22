using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.Engine;

namespace oldgoldmine_game.UI
{
    class Button
    {
        private Rectangle buttonArea;
        private SpriteText buttonText;

        private Texture2D normalButtonTexture;
        private Texture2D highlightedButtonTexture;

        private bool highlighted;


        public Button(Rectangle buttonArea, SpriteText text,
            Texture2D buttonNormal, Texture2D buttonHighlighted = null)
        {
            this.buttonArea = buttonArea;
            this.buttonText = text;
            this.normalButtonTexture = buttonNormal;
            this.highlightedButtonTexture = buttonHighlighted;

            if (buttonHighlighted == null)
                highlightedButtonTexture = normalButtonTexture;
        }

        public Button(Point position, Point size, SpriteText text,
            Texture2D buttonNormal, Texture2D buttonHighlighted = null)
        {
            this.buttonArea = new Rectangle(position, size);
            this.buttonText = text;
            this.normalButtonTexture = buttonNormal;
            this.highlightedButtonTexture = buttonHighlighted;

            if (buttonHighlighted == null)
                highlightedButtonTexture = normalButtonTexture;
        }

        public Button(int x, int y, int width, int height, SpriteText text,
            Texture2D buttonNormal, Texture2D buttonHighlighted = null)
        {
            this.buttonArea = new Rectangle(x, y, width, height);
            this.buttonText = text;
            this.normalButtonTexture = buttonNormal;
            this.highlightedButtonTexture = buttonHighlighted;

            if (buttonHighlighted == null)
                highlightedButtonTexture = normalButtonTexture;
        }

        public Button(Rectangle buttonArea, SpriteFont font, string text,
            Texture2D buttonNormal, Texture2D buttonHighlighted = null)
        {
            this.buttonArea = buttonArea;
            this.buttonText = new SpriteText(font, text,
                buttonArea.Center.ToVector2(), SpriteText.TextAlignment.MiddleCenter);
            this.normalButtonTexture = buttonNormal;
            this.highlightedButtonTexture = buttonHighlighted;

            if (buttonHighlighted == null)
                highlightedButtonTexture = normalButtonTexture;
        }

        public Button(Point position, Point size, SpriteFont font, string text,
            Texture2D buttonNormal, Texture2D buttonHighlighted = null)
        {
            this.buttonArea = new Rectangle(position, size);
            this.buttonText = new SpriteText(font, text, 
                buttonArea.Center.ToVector2(), SpriteText.TextAlignment.MiddleCenter);
            this.normalButtonTexture = buttonNormal;
            this.highlightedButtonTexture = buttonHighlighted;

            if (buttonHighlighted == null)
                highlightedButtonTexture = normalButtonTexture;
        }

        public Button(int x, int y, int width, int height, SpriteFont font, string text,
            Texture2D buttonNormal, Texture2D buttonHighlighted = null)
        {
            this.buttonArea = new Rectangle(x, y, width, height);
            this.buttonText = new SpriteText(font, text,
                buttonArea.Center.ToVector2(), SpriteText.TextAlignment.MiddleCenter);
            this.normalButtonTexture = buttonNormal;
            this.highlightedButtonTexture = buttonHighlighted;

            if (buttonHighlighted == null)
                highlightedButtonTexture = normalButtonTexture;
        }


        public void Update()
        {
            highlighted = buttonArea.Contains(InputManager.MousePosition);
        }


        public bool IsClicked()
        {
            return InputManager.MouseSingleLeftClick && buttonArea.Contains(InputManager.MousePosition);
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(highlighted ? highlightedButtonTexture : normalButtonTexture,
                destinationRectangle: buttonArea, Color.BurlyWood);

            buttonText.Draw(spriteBatch);
        }
    }
}
