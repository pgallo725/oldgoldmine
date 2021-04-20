using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.UI
{
    public class Image : IComponentUI
    {
        /// <summary>
        /// The enabled flag determines if the image is visible or hidden.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The pixel coordinates referring to the center of the Image.
        /// </summary>
        public Point Position
        {
            get { return imageArea.Center; }
            set { imageArea.Location = value - Size / new Point(2); }
        }

        /// <summary>
        /// The pixel size of the area covered by this Image.
        /// </summary>
        public Point Size
        {
            get { return imageArea.Size; }
            set
            {
                imageArea.Location = imageArea.Center - value / new Point(2);
                imageArea.Size = value;
            }
        }

        /// <summary>
        /// The Texture 2D drawn by this element.
        /// </summary>
        public Texture2D ImageTexture { get { return imageTexture; } set { imageTexture = value; } }
        private Texture2D imageTexture;

        /// <summary>
        /// The Rectangle area containing the image, with it's position and size values.
        /// </summary>
        public Rectangle Area { get { return imageArea; } set { imageArea = value; } }
        private Rectangle imageArea;

        /// <summary>
        /// The Color shade used to filter the Image sprite when drawing it.
        /// </summary>
        public Color Shade { get { return imageShade; } set { imageShade = value; } }
        private Color imageShade;


        /// <summary>
        /// Construct an Image UI element inside the specified Rectangle area.
        /// </summary>
        /// <param name="image">The Texture2D to draw as an image.</param>
        /// <param name="area">A Rectangle which will contain the image, defining it's position and size.</param>
        /// <param name="shade">The Color used to shade the drawn image, Color.White (default) preserves original colors.</param>
        public Image(Texture2D image, Rectangle area, Color shade = default)
        {
            this.imageTexture = image;
            this.imageArea = area;
            this.imageShade = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Construct an Image UI element with specific position and size values.
        /// </summary>
        /// <param name="image">The Texture2D to draw as an image.</param>
        /// <param name="position">The pixel coordinates referring to the center of the image.</param>
        /// <param name="size">The horizontal and vertical pixel dimensions of the image on screen.</param>
        /// <param name="shade">The Color used to shade the drawn image, Color.White (default) preserves original colors.</param>
        public Image(Texture2D image, Point position, Point size, Color shade = default)
            : this(image, new Rectangle(position - size / new Point(2), size), shade)
        {
        }

        /// <summary>
        /// Construct an Image UI element in the top-left corner of the screen, with it's original size.
        /// </summary>
        /// <param name="image">The Texture2D to draw as an image.</param>
        /// <param name="shade">The Color used to shade the drawn image, Color.White (default) preserves original colors.</param>
        public Image(Texture2D image, Color shade = default)
            : this(image, new Rectangle(0, 0, image.Width, image.Height), shade)
        {
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            if (Enabled && imageTexture != null)
                spriteBatch.Draw(imageTexture, imageArea, imageShade);
        }
    }
}
