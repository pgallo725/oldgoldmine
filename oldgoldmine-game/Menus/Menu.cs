using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    abstract class Menu
    {
        /// <summary>
        /// If this menu is a submenu of another entity, this field holds the reference to that parent menu
        /// </summary>
        protected Menu parent;

        /// <summary>
        /// The background image that will fill the entire window background while this menu is active
        /// </summary>
        protected Texture2D menuBackground;

        protected Image semiTransparencyLayer;      // to slightly obscure the background image and not interfere with items in the foreground


        /// <summary>
        /// Initialize the elements of this menu and their layout 
        /// </summary>
        /// <param name="viewport">A reference to the current application window (GraphicsDevice.Viewport)</param>
        /// <param name="background">The background image to use for this menu (or null)</param>
        /// <param name="parent">Specify the parent in case of nested menus (otherwise null)</param>
        public abstract void Initialize(Viewport viewport, Texture2D background, Menu parent = null);


        /// <summary>
        /// Prepare the menu to be shown on screen, by setting all the elements to their appropriate status
        /// </summary>
        public abstract void Show();


        /// <summary>
        /// In situations where there are nested menus, this can be called to go back to the parent from inside the submenu
        /// </summary>
        public abstract void CloseSubmenu();


        /// <summary>
        /// Update this menu's status, and run the code for all the elements it contains
        /// </summary>
        public abstract void Update();


        /// <summary>
        /// Draw the menu (and all of its elements) on the screen
        /// </summary>
        /// <param name="screen">A reference to the target GraphicsDevice of this render operation.</param>
        /// <param name="spriteBatch">A SpriteBatch object that will be used to draw the menu elements.
        /// It will Begin() and End() inside this call.</param>
        public abstract void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch);
    }

}
