using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MenuTutorial
{   
    /// <summary>
    /// Struck used to contain a Size
    /// </summary>
    struct Size
    {
        public int height { get; private set; }

        public int width { get; private set; } 

        public Size(int width, int height) : this()
        {
            this.height = height;
            this.width = width;

        }
    }

    /// <summary>
    /// This class contains all the functionality of the MainMenu
    /// </summary>
    class MainMenu
    {   
        /// <summary>
        /// Enum used to determine the current GameState
        /// </summary>
        enum GameState { mainMenu, enterName, inGame }

        /// <summary>
        /// The current state of the game
        /// </summary>
        GameState gameState;

        /// <summary>
        /// The name of the Player
        /// </summary>
        private string name = string.Empty;
        
        /// <summary>
        /// Determines if the letters should be caps or not
        /// </summary>
        private bool caps;

        /// <summary>
        /// A Collection that hold all GUI elements connected to the menu
        /// </summary>
        private List<List<GUIElement>> menus;

        /// <summary>
        /// Contains the last pressed key
        /// </summary>
        private Keys[] lastPressedKeys = new Keys[1];

        /// <summary>
        /// Spritefont used when  drawing the text entered by the player
        /// </summary>
        SpriteFont sf;

        
        public MainMenu()
        {   
            //Adds lists of GUI elements to the menu list
            menus = new List<List<GUIElement>>
            {   //MainMenu
                (new List<GUIElement> 
                { 
                    new GUIElement("menu"), 
                    new GUIElement("nameBtn"), 
                    new GUIElement("play") 
                }),
                //Enter name menu
                (new List<GUIElement> 
                { 
                    new GUIElement("name"), 
                    new GUIElement("done"), 
                })

            };
            //Sets the OnClick event on all menu items
            for (int i = 0; i < menus.Count; i++)
            {
                foreach (GUIElement button in menus[i])
                {
                    button.clickEvent += OnClick;
                } 
            }

        }
        /// <summary>
        /// Loads all content in the menu
        /// </summary>
        /// <param name="content">ContentManager</param>
        /// <param name="windowSize">the size of the GameWindow</param>
        public void LoadContent(ContentManager content, Size windowSize)
        {   
            //Loads the content of our spritefont
            sf = content.Load<SpriteFont>("MyFont");

            //Loads the content of all other GUI elements
            for (int i = 0; i < menus.Count; i++)
            {
                foreach (GUIElement button in menus[i])
                {
                    button.LoadContent(content);
                    button.CenterElement(windowSize);
                }
            }
            //Sets offsets of the buttons
            menus[0].Find(x => x.ElementName == "play").MoveElement(0, -100);
            menus[0].Find(x => x.ElementName == "nameBtn").MoveElement(0, -40);
            menus[1].Find(x => x.ElementName == "done").MoveElement(0, 60);
            
        }
        /// <summary>
        /// Updates all our GUI element(Mainly checks if any button is pressed)
        /// </summary>
        public void Update()
        {   
            //Update each element according to the current gameState
            switch (gameState)
            {
                case GameState.mainMenu:
                    foreach (GUIElement button in menus[0]) //MainMenu
                    {
                        button.Update();
                    }
                    break;
                case GameState.enterName:
                    foreach (GUIElement button in menus[1])//EnterName menu
                    {
                        GetKeys();
                        button.Update();
                    }
                    break;
                case GameState.inGame: //Ingame GUI
                    break;
            }
        }

        /// <summary>
        /// Draws all GUI elements
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {   
            //Draws each element according to the current gameState
            switch (gameState)
            {
                case GameState.mainMenu:
                    foreach (GUIElement button in menus[0]) //MainMenu
                    {
                        button.Draw(spriteBatch);
                    }
                    break;
                case GameState.enterName:
                    foreach (GUIElement button in menus[1])//EnterName menu
                    {   
                        spriteBatch.DrawString(sf, name, new Vector2(305, 300), Color.Black);
                        button.Draw(spriteBatch);
                    }
                    break;
                case GameState.inGame://Ingame GUI
                    break;
            }

        }

        /// <summary>
        /// Method called every time a GUI element is clicked
        /// </summary>
        /// <param name="element"></param>
        public void OnClick(string element)
        {
            if (element == "play")//PlayButton
            {
                gameState = GameState.inGame;
            }
            if (element == "nameBtn")//EnterName button
            {
                gameState = GameState.enterName;
            }
            if (element == "done")//Done button
            {
                gameState = GameState.mainMenu;
            }
        }

        private void GetKeys()
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            //check if any of the previous update's keys are no longer pressed
            foreach (Keys key in lastPressedKeys)
            {
                if (!pressedKeys.Contains(key))
                    OnKeyUp(key);
            }

            //check if the currently pressed keys were already pressed
            foreach (Keys key in pressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                    OnKeyDown(key);
            }
            //save the currently pressed keys so we can compare on the next update
            lastPressedKeys = pressedKeys;
        }

        private void OnKeyDown(Keys key)
        {
            //do stuff
            if (key == Keys.Back && name.Length > 0) //Removes a letter from the name if there is a letter to remove
            {
                name = name.Remove(name.Length - 1);
            }
            else if (key == Keys.LeftShift || key == Keys.RightShift)//Sets caps to true if a shift key is pressed
            {
                caps = true;
            }
            else if (!caps && name.Length < 16) //If the name isn't too long, and !caps the letter will be added without caps
            {
                if (key == Keys.Space)
                {
                    name += " ";
                }
                else
                {
                    name += key.ToString().ToLower();
                }
            }
            else if (name.Length < 16) //Adds the letter to the name in CAPS
            {
                name += key.ToString();
            }
        }

        private void OnKeyUp(Keys key)
        {
            //Sets caps to false if one of the shift keys goes up
            if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                caps = false;
            }
        }
    }
}
