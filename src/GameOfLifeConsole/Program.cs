using System;
using System.Threading;

using GameOfLife;

namespace GameOfLifeConsole
{
    class Program
    {
        static GameOfLifeSession game;

        static void Main(string[] args)
        {
            Console.Title = "Game Of Life";

            game = new GameOfLifeSession();

            // TODO Should convert the setup stuff to a separate method
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WELCOME TO THE GAME OF LIFE\n");
            //TODO Insert explanation of simulation here
            Console.WriteLine("Set Simulation Options: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            // TODO Add error testing to the simulation options

            Console.Write("Enter the number of rows (1-30):     ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            game.Rows = int.Parse(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter the number of columns (1-30):  ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            game.Columns = int.Parse(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter refresh rate in seconds (1-5): ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            game.CycleTime = 1000 * double.Parse(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\nPRESS <CTRL-C> TO END THE SIMULATION\n");
            Console.Write("\nPRESS <ENTER> TO START");
            Console.CursorVisible = false;            
            Console.ReadLine();

            Console.CancelKeyPress += (sender, args) =>
            {
                game.Stop();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n==> End of simulation.");
            };

            game.NextCycle += Game_Update;

            game.Start();
            Thread.Sleep(Timeout.Infinite);
        }

        private static void Game_Update(object sender, NextCycleEventArgs e)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);

            for (int row = 0; row < game.Rows; row++)
            {
                for (int column = 0; column < game.Columns; column++)
                {
                    Status cell = e.NextCycle[row, column];
                    Console.ForegroundColor = cell == Status.Alive ? ConsoleColor.Green : ConsoleColor.DarkRed;
                    Console.Write(cell == Status.Alive ? "*" : ".");
                }
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nResult of cycle:       {game.CycleCounter}");
            Console.WriteLine($"\nNumber of cells alive: {game.AliveCounter}");
        }
    }
}
