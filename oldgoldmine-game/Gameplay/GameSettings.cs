namespace OldGoldMine.Gameplay
{
    public struct GameSettings
    {
        /// <summary>
        /// The multiplier applied to the game score based on the selected difficulty settings.
        /// </summary>
        public float ScoreMultiplier 
        { 
            get { return 1f + (Difficulty - 1) * 0.5f + (StartSpeed - 20) / 10 * 0.05f; }
        }

        /// <summary>
        /// The starting speed of the player in the level, in Km/h.
        /// </summary>
        public float StartSpeed { get; set; }

        /// <summary>
        /// The difficulty index selected by the player for the current game (0 = Easy, 1 = Medium, 2 = Hard).
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// User-provided of randomly generated seed for procedural level generation.
        /// </summary>
        public long Seed { get; set; }

        /// <summary>
        /// The index of the minecart model selected by the player for the current game.
        /// </summary>
        public int Cart { get; set; }

        /// <summary>
        /// Define the difficulty and customization parameters for a run of the game.
        /// </summary>
        /// <param name="startSpeed">The starting speed of the player in the level, in Km/h.</param>
        /// <param name="difficulty">The difficulty index for the game (0 = Easy, 1 = Medium, 2 = Hard).</param>
        /// <param name="seed">User-provided of randomly generated seed for procedural level generation.</param>
        /// <param name="cart">The index of the minecart model for the current game.</param>
        public GameSettings(int startSpeed, int difficulty, long seed, int cart)
        {
            this.StartSpeed = startSpeed;
            this.Difficulty = difficulty;
            this.Seed = seed;
            this.Cart = cart;
        }
    }
}
