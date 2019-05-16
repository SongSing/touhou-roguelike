using System;

namespace moth
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Root())
                game.Run();
        }
    }
}
