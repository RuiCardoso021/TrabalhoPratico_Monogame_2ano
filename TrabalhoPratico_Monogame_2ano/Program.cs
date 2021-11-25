using System;

namespace TrabalhoPratico_Monogame_2ano
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}