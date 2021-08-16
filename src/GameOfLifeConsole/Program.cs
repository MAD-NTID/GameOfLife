using System;
using System.Threading;
using GameOfLife;

namespace GameOfLifeConsole
{
    class Program
    {        
        static void Main(string[] args)
        {
            // Step 4 | Pre-Game Preparation
            GameOfLifeSession game = new GameOfLifeSession(); // 4.1

            Console.Title = "Game Of Life"; // 4.2

            Console.ForegroundColor = ConsoleColor.Yellow; // 4.3
            Console.WriteLine("WELCOME TO THE GAME OF LIFE\n"); // 4.3
            
            // Step 5 | Getting User Information
            Console.WriteLine("Set Simulation Options: "); // 5.1

            Console.ForegroundColor = ConsoleColor.Cyan; // 5.2
            Console.Write("Enter the number of rows (1-30):     "); // 5.2
            Console.ForegroundColor = ConsoleColor.Magenta; // 5.2
            game.Rows = int.Parse(Console.ReadLine()); // 5.2

            Console.ForegroundColor = ConsoleColor.Cyan; // 5.3
            Console.Write("Enter the number of columns (1-30):  "); // 5.3
            Console.ForegroundColor = ConsoleColor.Magenta; // 5.3
            game.Columns = int.Parse(Console.ReadLine()); // 5.3

            Console.ForegroundColor = ConsoleColor.Cyan; // 5.4
            Console.Write("Enter refresh rate in seconds (1-5): "); // 5.4
            Console.ForegroundColor = ConsoleColor.Magenta; // 5.4
            game.CycleTime = 1000 * double.Parse(Console.ReadLine()); // 5.4

            // Step 6 | Adding Update Logic
            game.NextCycle += Game_Update; // 6.1

            // Step 7 | Starting / Stopping the Game
            Console.ForegroundColor = ConsoleColor.Yellow; // 7.1
            Console.WriteLine("\n\nPRESS <CTRL + C> TO END THE SIMULATION\n"); // 7.1
            Console.Write("\nPRESS <ENTER> TO START"); // 7.1
            Console.CursorVisible = false; // 7.2
            Console.ReadLine(); // 7.2
            Console.CancelKeyPress += (sender, args) => // 7.3
            {
                game.Stop(); // 7.4
                Console.ForegroundColor = ConsoleColor.Yellow; // 7.4
                Console.WriteLine("\n==> End of simulation."); // 7.4
            }; // 7.3
            game.Start(); // 7.5
            Thread.Sleep(Timeout.Infinite); // 7.5
        }

        private static void Game_Update(GameOfLifeSession game, Status[,] nextCycle)  // 6.1
        {  // 6.1
            Console.Clear(); // 6.2           

            for (int row = 0; row < game.Rows; row++) // 6.3
            { // 6.3
                for (int column = 0; column < game.Columns; column++) // 6.4
                {
                    Status cell = nextCycle[row, column]; // 6.5
                    Console.ForegroundColor = cell == Status.Alive ? ConsoleColor.Green : ConsoleColor.DarkRed; // 6.5
                    Console.Write(cell == Status.Alive ? "*" : "."); // 6.5
                } // 6.4
                Console.WriteLine(); // 6.6
            } // 6.3
            Console.ForegroundColor = ConsoleColor.Yellow; // 6.7
            Console.WriteLine("\nResult of cycle:       " + game.CycleCounter); // 6.7
            Console.WriteLine("\nNumber of cells alive: " + game.AliveCounter); // 6.7
        }  // 6.1
    }
}
