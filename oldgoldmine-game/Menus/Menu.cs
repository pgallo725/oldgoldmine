using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    abstract class Menu
    {
        protected Vector2 buttonSize = new Vector2(400, 120);
        protected int elementSeparation = 70;

        protected Texture2D menuBackground;


        public abstract void Initialize(GraphicsDevice device, Texture2D background,
            SpriteFont font, Texture2D normalButton, Texture2D highlightedButton);


        public abstract void Update();


        public abstract void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch);
    }

}
