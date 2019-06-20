using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using oldgoldmine_game.UI;

namespace oldgoldmine_game.Menus
{
    class MainMenu : Menu
    {

        private Button playButton;
        private Button exitButton;

        private SpriteText highscoreText;


        public override void Initialize(GraphicsDevice device, Texture2D background)
        {
            this.menuBackground = background;

            // Main menu layout setup

            playButton = new Button(
                device.Viewport.Bounds.Center.ToVector2() - new Vector2(0, elementSeparation), buttonSize, 
                OldGoldMineGame.resources.menuButtonFont, "PLAY", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);

            exitButton = new Button(
                device.Viewport.Bounds.Center.ToVector2() + new Vector2(0, elementSeparation), buttonSize,
                OldGoldMineGame.resources.menuButtonFont, "QUIT", Color.White,
                OldGoldMineGame.resources.menuButtonTextures);


            Vector2 highscoreTextPosition = new Vector2(device.Viewport.Width / 2,
                (device.Viewport.Height + buttonSize.Y + 3 * elementSeparation) / 2);

            highscoreText = new SpriteText(OldGoldMineGame.resources.menuButtonFont, "Highscore: " + OldGoldMineGame.BestScore,
                Color.LightGoldenrodYellow, highscoreTextPosition, SpriteText.TextAnchor.MiddleCenter);
        }


        public override void Update()
        {
            playButton.Update();
            exitButton.Update();

            if (playButton.IsClicked())
                OldGoldMineGame.Application.NewGame();
            else if (exitButton.IsClicked())
                OldGoldMineGame.Application.Exit();
        }


        public override void Draw(in GraphicsDevice screen, in SpriteBatch spriteBatch)
        {
            screen.Clear(Color.Black);

            spriteBatch.Begin();

            if (menuBackground != null)
                spriteBatch.Draw(menuBackground, screen.Viewport.Bounds, Color.White);

            playButton.Draw(spriteBatch);
            exitButton.Draw(spriteBatch);

            highscoreText.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void Show()
        {
            highscoreText.Text = "Highscore: " + OldGoldMineGame.BestScore;
        }
    }
}
