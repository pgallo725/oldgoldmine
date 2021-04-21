using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// A texture-like object made up of a single colored pixel, that can be scaled to
    /// any size without quality loss thanks to its nature, useful for UI or other purposes.
    /// </summary>
    public class SolidColorTexture : Texture2D
    {
        private Color _color;

        /// <summary>
        /// The solid color of the texture.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                if (value != _color)
                {
                    _color = value;
                    SetData(new Color[] { _color });
                }
            }
        }


        /// <summary>
        /// Creates a 1x1 pixel texture and fill it with a solid color.
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice object for which the texture is created.</param>
        /// <param name="color">The color of the resulting texture image.</param>
        public SolidColorTexture(GraphicsDevice graphicsDevice, Color color)
            : base(graphicsDevice, 1, 1)
        {
            Color = color;
        }

    }
}
