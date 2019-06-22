using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace oldgoldmine_game.UI
{
    class SpriteText : IComponentUI
    {

        public enum TextAnchor
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


        private bool show = true;
        private string text;
        private SpriteFont font;
        private Color color;
        private Vector2 position;
        private Vector2 alignmentOffset;
        private TextAnchor anchorPoint;


        /// <summary>
        /// The text string rendered by this element
        /// </summary>
        public string Text { get { return text; } set { text = value; UpdateAlignmentOffset(); } }

        /// <summary>
        /// The SpriteFont used to render the text
        /// </summary>
        public SpriteFont Font { get { return font; } set { font = value; UpdateAlignmentOffset(); } }

        /// <summary>
        /// The color of the label's text
        /// </summary>
        public Color Color { get { return color; } set { color = value; } }

        /// <summary>
        /// The enabled flag determines if the label is visible or hidden
        /// </summary>
        public bool Enabled { get { return show; } set { show = value; } }

        /// <summary>
        /// The position of the anchor point of this text element
        /// </summary>
        public Vector2 Position { get { return position; } set { position = value; } }

        /// <summary>
        /// Defines how the text is anchored to the point defined by the Position value
        /// </summary>
        public TextAnchor AnchorPoint { get { return anchorPoint; } set { anchorPoint = value; UpdateAlignmentOffset(); } }

        
        /// <summary>
        /// Construct an empty SpriteText element, at default coordinates
        /// </summary>
        /// <param name="font">The SpriteFont used to render the text.</param>
        public SpriteText(SpriteFont font)
        {
            this.font = font;
            this.text = string.Empty;
            this.position = Vector2.Zero;
            this.alignmentOffset = Vector2.Zero;
            this.anchorPoint = TextAnchor.MiddleCenter;
            this.color = Color.White;
        }

        /// <summary>
        /// Construct a SpriteText element with specified font, text and position/alignment
        /// </summary>
        /// <param name="font">The SpriteFont used to render the text.</param>
        /// <param name="text">The text to be shown in this element.</param>
        /// <param name="position">The position of the anchor point of this text.</param>
        /// <param name="anchorPoint">The type of anchor point specified by Position.</param>
        public SpriteText(SpriteFont font, string text, Vector2 position, TextAnchor anchorPoint = TextAnchor.MiddleCenter)
        {
            this.font = font;
            this.text = text;
            this.position = position;
            this.AnchorPoint = anchorPoint;
            this.color = Color.White;
        }

        /// <summary>
        /// Construct a SpriteText element with specified font, color, text and position/alignment
        /// </summary>
        /// <param name="font">The SpriteFont used to render the text.</param>
        /// <param name="text">The text to be shown in this element.</param>
        /// <param name="color">The color in which the text will be rendered.</param>
        /// <param name="position">The position of the anchor point of this text.</param>
        /// <param name="anchorPoint">The type of anchor point specified by Position.</param>
        public SpriteText(SpriteFont font, string text, Color color, Vector2 position, TextAnchor anchorPoint = TextAnchor.MiddleCenter)
        {
            this.font = font;
            this.text = text;
            this.position = position;
            this.AnchorPoint = anchorPoint;
            this.color = color;
        }


        private void UpdateAlignmentOffset()
        {
            if (font == null)
                return;
            Vector2 textSize = font.MeasureString(text);

            // Calculate the offset needed to properly align the text
            switch (anchorPoint)
            {
                case TextAnchor.TopLeft:
                    alignmentOffset = Vector2.Zero;
                    break;

                case TextAnchor.TopCenter:
                    alignmentOffset = new Vector2(-textSize.X / 2, 0f);
                    break;

                case TextAnchor.TopRight:
                    alignmentOffset = new Vector2(-textSize.X, 0f);
                    break;

                case TextAnchor.MiddleLeft:
                    alignmentOffset = new Vector2(0f, -textSize.Y / 2);
                    break;

                case TextAnchor.MiddleCenter:
                    alignmentOffset = -textSize / 2;
                    break;

                case TextAnchor.MiddleRight:
                    alignmentOffset = new Vector2(-textSize.X, -textSize.Y / 2);
                    break;

                case TextAnchor.BottomLeft:
                    alignmentOffset = new Vector2(0f, -textSize.Y);
                    break;

                case TextAnchor.BottomCenter:
                    alignmentOffset = new Vector2(textSize.X / 2, -textSize.Y);
                    break;

                case TextAnchor.BottomRight:
                    alignmentOffset = -textSize;
                    break;
            }
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            if (!show)      // Do not draw the text if the element is flagged as disabled
                return;

            if (font != null)
                spriteBatch.DrawString(font, text, position + alignmentOffset, color);
        }

    }
}
