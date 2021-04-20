using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.UI
{
    /// <summary>
    /// Common class interface for all components of the menus and in-game UI.
    /// </summary>
    public interface IComponentUI
    {
        /// <summary>
        /// Determines if this UI element is active and interactable or disabled/hidden.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The pixel coordinates referring to the center of this UI element.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// The size of this UI element, in pixels.
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        /// Draw the UI element on the screen.
        /// </summary>
        /// <param name="spriteBatch">An opened SpriteBatch object that will be used to draw the element.</param>
        public void Draw(in SpriteBatch spriteBatch);
    }
}
