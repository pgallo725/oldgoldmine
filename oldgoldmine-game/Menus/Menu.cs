using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace oldgoldmine_game.Menus
{
    abstract class Menu
    {
        protected SpriteFont menuFont;
        protected Texture2D menuBackground;

        protected Texture2D buttonTextureNormal;
        protected Texture2D buttonTextureHighlighted;

        protected MouseState lastMouseState;
        protected MouseState currentMouseState;


        public abstract void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton);


        public abstract void Update(in OldGoldMineGame application);


        public abstract void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch);
    }

}
