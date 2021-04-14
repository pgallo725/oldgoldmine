using System;

namespace OldGoldMine
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
