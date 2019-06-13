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
            set { imageArea.Size = value.ToPoint(); }       // TODO: test how size scales
        }

        private Texture2D imageTexture;
        private Rectangle imageArea;


        /// <summary>
        /// The Texture 2D drawn by this element
        /// </summary>
        public Texture2D ImageTexture { get { return imageTexture; } set { imageTexture = value; } }
        /// <summary>
        /// The Rectangle area containing the image, with it's position and size values
        /// </summary>
        public Rectangle DrawArea { get { return imageArea; } set { imageArea = value; } }


        /// <summary>
        /// Construct an Image UI element in the top-left corner of the screen, with it's original size
        /// </summary>
        /// <param name="image">The Texture 2D to draw as an image.</param>
        public Image(Texture2D image)
        {
            this.imageTexture = image;
            this.imageArea = new Rectangle(0, 0, image.Width, image.Height);
        }

        /// <summary>
        /// Construct an Image UI element inside the specified Rectangle area
        /// </summary>
        /// <param name="image">The Texture 2D for the image.</param>
        /// <param name="area">A Rectangle which will contain the image, defining it's position and size.</param>
        public Image(Texture2D image, Rectangle area)
        {
            this.imageTexture = image;
            this.imageArea = area;
        }

        /// <summary>
        /// Construct an Image UI element with specific position and size values
        /// </summary>
        /// <param name="image">The Texture 2D for the image.</param>
        /// <param name="position">The pixel coordinates referring to the center of the image.</param>
        /// <param name="position">The horizontal and vertical pixel dimensions of the image on screen.</param>
        public Image(Texture2D image, Vector2 position, Vector2 size)
        {
            this.imageTexture = image;
            this.imageArea = new Rectangle((position + size/2).ToPoint(), size.ToPoint());
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(imageTexture, imageArea, Color.White);
        }
    }
}
