using System;

namespace oldgoldmine_game
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new OldGoldMineGame();
            game.Run();
        }
    }
}
