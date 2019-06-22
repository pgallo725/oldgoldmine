using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace oldgoldmine_game.UI
{
    public interface IComponentUI
    {
        /// <summary>
        /// The pixel coordinates referring to the center of this UI element
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Flag which determines if this UI element is active and interactable or disabled/hidden
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Draw the UI element on the screen
        /// </summary>
        /// <param name="spriteBatch">An opened SpriteBatch object that will be used to draw the element.</param>
        void Draw(in SpriteBatch spriteBatch);
    }
}
