using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.UI
{
    public class SpriteText : IComponentUI
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


        /// <summary>
        /// The enabled flag determines if the label is visible or hidden.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The position of the anchor point of this text element.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// The size occupied by the SpriteText element, in pixels.
        /// <para>NOTE: setting this value has no effect.</para>
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        /// The text string rendered by this element.
        /// </summary>
        public string Text { get { return textString; } set { textString = value; UpdateAlignmentOffset(); } }
        private string textString;

        /// <summary>
        /// The SpriteFont used to render the text.
        /// </summary>
        public SpriteFont Font { get { return textFont; } set { textFont = value; UpdateAlignmentOffset(); } }
        private SpriteFont textFont;

        /// <summary>
        /// The color of the label's text.
        /// </summary>
        public Color Color { get { return textColor; } set { textColor = value; } }
        private Color textColor;

        /// <summary>
        /// Defines how the text is anchored to the point defined by the Position value.
        /// </summary>
        public TextAnchor Anchor { get { return textAnchor; } set { textAnchor = value; UpdateAlignmentOffset(); } }
        private TextAnchor textAnchor;
        private Vector2 textOffset;


        /// <summary>
        /// Construct a SpriteText element with specified font, color, text and position/alignment.
        /// </summary>
        /// <param name="font">The SpriteFont used to render the text.</param>
        /// <param name="text">The text to be shown in this element.</param>
        /// <param name="color">The color in which the text will be rendered.</param>
        /// <param name="position">The position of the anchor point of this text.</param>
        /// <param name="anchor">The type of anchor point specified by Position.</param>
        public SpriteText(SpriteFont font, string text, Color color, Point position, TextAnchor anchor = TextAnchor.MiddleCenter)
        {
            this.textFont = font;
            this.textString = text;
            this.Position = position;
            this.Anchor = anchor;     // Internally calculates the alignment offset
            this.textColor = color;
        }


        private void UpdateAlignmentOffset()
        {
            if (textFont == null)
                return;
            Vector2 textSize = textFont.MeasureString(textString);

            // Calculate the offset needed to properly align the text
            switch (textAnchor)
            {
                case TextAnchor.TopLeft:
                    textOffset = Vector2.Zero;
                    break;

                case TextAnchor.TopCenter:
                    textOffset = new Vector2(-textSize.X / 2, 0f);
                    break;

                case TextAnchor.TopRight:
                    textOffset = new Vector2(-textSize.X, 0f);
                    break;

                case TextAnchor.MiddleLeft:
                    textOffset = new Vector2(0f, -textSize.Y / 2);
                    break;

                case TextAnchor.MiddleCenter:
                    textOffset = -textSize / 2;
                    break;

                case TextAnchor.MiddleRight:
                    textOffset = new Vector2(-textSize.X, -textSize.Y / 2);
                    break;

                case TextAnchor.BottomLeft:
                    textOffset = new Vector2(0f, -textSize.Y);
                    break;

                case TextAnchor.BottomCenter:
                    textOffset = new Vector2(textSize.X / 2, -textSize.Y);
                    break;

                case TextAnchor.BottomRight:
                    textOffset = -textSize;
                    break;
            }
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            if (Enabled && textFont != null)
                spriteBatch.DrawString(textFont, textString, Position.ToVector2() + textOffset, textColor);
        }

    }
}
