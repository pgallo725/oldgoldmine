using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace oldgoldmine_game.UI
{
    class SpriteText : ComponentUI
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

        public string Text { get { return text; } set { text = value; UpdateAlignmentOffset(); } }
        public SpriteFont Font { get { return font; } set { font = value; UpdateAlignmentOffset(); } }
        public Color Color { get { return color; } set { color = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public TextAlignment Alignment { get { return alignment; } set { alignment = value; UpdateAlignmentOffset(); } }

        
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


        private void UpdateAlignmentOffset()
        {
            if (font == null)
                return;
            Vector2 textSize = font.MeasureString(text);

            // Calculate the offset needed to properly align the text
            switch (alignment)
            {
                case TextAlignment.TopLeft:
                    alignmentOffset = Vector2.Zero;
                    break;

                case TextAlignment.TopCenter:
                    alignmentOffset = new Vector2(-textSize.X / 2, 0f);
                    break;

                case TextAlignment.TopRight:
                    alignmentOffset = new Vector2(-textSize.X, 0f);
                    break;

                case TextAlignment.MiddleLeft:
                    alignmentOffset = new Vector2(0f, -textSize.Y / 2);
                    break;

                case TextAlignment.MiddleCenter:
                    alignmentOffset = -textSize / 2;
                    break;

                case TextAlignment.MiddleRight:
                    alignmentOffset = new Vector2(-textSize.X, -textSize.Y / 2);
                    break;

                case TextAlignment.BottomLeft:
                    alignmentOffset = new Vector2(0f, -textSize.Y);
                    break;

                case TextAlignment.BottomCenter:
                    alignmentOffset = new Vector2(textSize.X / 2, -textSize.Y);
                    break;

                case TextAlignment.BottomRight:
                    alignmentOffset = -textSize;
                    break;
            }
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            if (font == null)
                return;

            spriteBatch.DrawString(font, text, position + alignmentOffset, color);
        }

    }
}
