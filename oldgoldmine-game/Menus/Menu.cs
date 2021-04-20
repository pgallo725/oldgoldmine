using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldGoldMine.Menus
{
    /// <summary>
    /// Common class interface for all in-game menus.
    /// </summary>
    abstract class Menu
    {
        /// <summary>
        /// If this menu is a submenu of another entity, this is the reference to the parent menu.
        /// </summary>
        protected Menu parent;

        /// <summary>
        /// The background image that will fill the entire window background while this menu is active.
        /// </summary>
        protected Texture2D background;

        // This can be used to slightly obscure or cover the background image
        protected Texture2D middleLayer;

        /// <summary>
        /// The reference size for buttons in this menu, also used for layout calculations.
        /// </summary>
        protected Point buttonSize;


        /// <summary>
        /// Contructs a Menu object by defining its characteristics.
        /// </summary>
        /// <param name="background">The background image to use for this menu (or null).</param>
        /// <param name="middleLayer">The texture to be used as a middle layer between background and foreground (or null).</param>
        /// <param name="buttonSize">Reference size for buttons in this menu, also used for layout calculations.</param>
        /// <param name="parent">Specify the parent in case of nested menus (otherwise null).</param>
        public Menu(Texture2D background, Texture2D middleLayer, Point buttonSize, Menu parent = null)
        {
            this.parent = parent;
            this.background = background;
            this.middleLayer = middleLayer;
            this.buttonSize = buttonSize;
        }


        /// <summary>
        /// Caluculates the layout of UI elements in the menu, depending on the viewport size.
        /// </summary>
        protected abstract void Layout();


        /// <summary>
        /// Prepare the menu to be shown on screen, applying the layout and setting the appropriate status to all elements.
        /// </summary>
        public abstract void Show();


        /// <summary>
        /// Update this menu's status in the current frame, and run the code for all the elements it contains.
        /// </summary>
        public abstract void Update();


        /// <summary>
        /// Draw the menu (and all of its elements) on the screen.
        /// </summary>
        /// <param name="screen">A reference to the target GraphicsDevice of this render operation.</param>
        /// <param name="spriteBatch">A SpriteBatch object that will be used to draw the menu elements.
        /// It will Begin() and End() inside this call.</param>
        public abstract void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch);
    }

}
