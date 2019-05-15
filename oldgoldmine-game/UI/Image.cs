using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace oldgoldmine_game.UI
{
    class Image
    {

        private Texture2D imageTexture;
        private Rectangle drawArea;

        public Texture2D ImageTexture { get { return imageTexture; } set { imageTexture = value; } }
        public Rectangle DrawArea { get { return drawArea; } set { drawArea = value; } }


        public Image(Texture2D image)
        {
            this.imageTexture = image;
            this.drawArea = new Rectangle(0, 0, image.Width, image.Height);
        }

        public Image(Texture2D image, Rectangle area)
        {
            this.imageTexture = image;
            this.drawArea = area;
        }

        public Image(Texture2D image, Point center, Vector2 size)
        {
            this.imageTexture = image;
            this.drawArea = new Rectangle(center.X, center.Y, (int)size.X, (int)size.Y);
        }


        public void Draw(in SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(imageTexture, destinationRectangle: drawArea, color: null);
        }
    }
}
