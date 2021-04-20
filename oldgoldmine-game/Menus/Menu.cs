using Microsoft.Xna.Framework.Graphics;
using OldGoldMine.UI;

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

        protected Image transparencyLayer;      // to slightly obscure the background image and not interfere with the foreground


        /// <summary>
        /// Initialize the elements of this menu and their layout.
        /// </summary>
        /// <param name="viewport">A reference to the current application window (GraphicsDevice.Viewport).</param>
        /// <param name="background">The background image to use for this menu (or null).</param>
        /// <param name="parent">Specify the parent in case of nested menus (otherwise null).</param>
        public abstract void Initialize(Viewport viewport, Texture2D background, Menu parent = null);


        /// <summary>
        /// Caluculates the layout of UI elements in the menu, depending on the viewport size.
        /// </summary>
        protected abstract void Layout();


        /// <summary>
        /// Prepare the menu to be shown on screen, applying the layout and setting the appropriate status to all elements.
        /// </summary>
        public abstract void Show();


        /// <summary>
        /// In the case of nested menus, this can be called from inside the submenu to go back to its parent.
        /// </summary>
        public abstract void CloseSubmenu();


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
