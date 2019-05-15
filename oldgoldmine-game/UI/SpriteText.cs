using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace oldgoldmine_game.UI
{
    class SpriteText
    {

        public enum TextAlignment
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }


        private string text;
        private SpriteFont font;
        private Color color;
        private Vector2 position;
        private Vector2 alignmentOffset;
        private TextAlignment alignment;

        public string Text { get { return text; } set { text = value; } }
        public SpriteFont Font { get { return font; } set { font = value; } }
        public Color Color { get { return color; } set { color = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public TextAlignment Alignment { get { return alignment; } set { alignment = value; } }

        
        public SpriteText(SpriteFont font)
        {
            this.font = font;
            this.text = string.Empty;
            this.position = Vector2.Zero;
            this.alignmentOffset = Vector2.Zero;
            this.alignment = TextAlignment.TopLeft;
            this.color = Color.White;
        }

        public SpriteText(SpriteFont font, string text, Vector2 position, TextAlignment alignment = TextAlignment.TopLeft)
        {
            this.font = font;
            this.text = text;
            this.position = position;
            this.Alignment = alignment;
            this.color = Color.White;
        }

        public SpriteText(SpriteFont font, string text, Color color, Vector2 position, TextAlignment alignment = TextAlignment.TopLeft)
        {
            this.font = font;
            this.text = text;
            this.position = position;
            this.Alignment = alignment;
            this.color = color;
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            if (font == null)
                return;

            spriteBatch.DrawString(font, text, position + alignmentOffset, color);
        }

    }
}
