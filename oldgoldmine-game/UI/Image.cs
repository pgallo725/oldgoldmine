using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace oldgoldmine_game.UI
{
    class Image : IComponentUI
    {

        /// <summary>
        /// The pixel coordinates referring to the center of the Image
        /// </summary>
        public Vector2 Position
        {
            get { return imageArea.Center.ToVector2(); }
            set { imageArea.Location = value.ToPoint() - imageArea.Size / new Point(2); }
        }

        /// <summary>
        /// The pixel size of the area covered by this Image
        /// </summary>
        public Vector2 Size
        {
            get { return imageArea.Size.ToVector2(); }
            set
            {
                Point oldPosition = imageArea.Center;
                imageArea.Size = value.ToPoint();
                imageArea.Location = oldPosition - imageArea.Size / new Point(2);
            }
        }

        private bool imageVisible = true;
        private Texture2D imageTexture;
        private Rectangle imageArea;
        private Color imageColor;


        /// <summary>
        /// The enabled flag determines if the image is visible or hidden
        /// </summary>
        public bool Enabled { get { return imageVisible; } set { imageVisible = value; } }

        /// <summary>
        /// The Texture 2D drawn by this element
        /// </summary>
        public Texture2D ImageTexture { get { return imageTexture; } set { imageTexture = value; } }

        /// <summary>
        /// The Color shade used to filter the Image sprite when drawing it
        /// </summary>
        public Color Shade { get { return imageColor; } set { imageColor = value; } }

        /// <summary>
        /// The Rectangle area containing the image, with it's position and size values
        /// </summary>
        public Rectangle DrawArea { get { return imageArea; } set { imageArea = value; } }


        /// <summary>
        /// Construct an Image UI element in the top-left corner of the screen, with it's original size
        /// </summary>
        /// <param name="image">The Texture 2D to draw as an image.</param>
        /// <param name="shade">The Color used to shade the image when drawing it one screen (default = Color.White and preverves original colors)</param>
        public Image(Texture2D image, Color shade = default)
        {
            this.imageTexture = image;
            this.imageArea = new Rectangle(0, 0, image.Width, image.Height);
            this.imageColor = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Construct an Image UI element inside the specified Rectangle area
        /// </summary>
        /// <param name="image">The Texture 2D for the image.</param>
        /// <param name="area">A Rectangle which will contain the image, defining it's position and size.</param>
        /// <param name="shade">The Color used to shade the image when drawing it (Color.White acts the same as default)</param>
        public Image(Texture2D image, Rectangle area, Color shade = default)
        {
            this.imageTexture = image;
            this.imageArea = area;
            this.imageColor = shade == default ? Color.White : shade;
        }

        /// <summary>
        /// Construct an Image UI element with specific position and size values
        /// </summary>
        /// <param name="image">The Texture 2D for the image.</param>
        /// <param name="position">The pixel coordinates referring to the center of the image.</param>
        /// <param name="size">The horizontal and vertical pixel dimensions of the image on screen.</param>
        /// <param name="shade">The Color used to shade the image when drawing it on screen (Color.White acts the same as default)</param>
        public Image(Texture2D image, Vector2 position, Vector2 size, Color shade = default)
        {
            this.imageTexture = image;
            this.imageArea = new Rectangle((position - size/2).ToPoint(), size.ToPoint());
            this.imageColor = shade == default ? Color.White : shade;
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            if (imageVisible && imageTexture != null)
                spriteBatch.Draw(imageTexture, imageArea, imageColor);
        }
    }
}
