/*
 * Author: Mighty Mark 7/27/2021
 * With his trusty side-kick encyclopedia Chase...
 */

using System;
using System.Threading;

namespace GameOfLiveV2 {
    class Program {
        static int Rows;
        static int Columns;

        static bool runSimulation;

        static int cycleTime = 5000;
        static int cycle;

        static readonly Random rnd = new();

        static void Main(string[] args) {
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
            Rows = int.Parse(Console.ReadLine());
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter the number of columns (1-30):  ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Columns = int.Parse(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter refresh rate in seconds (1-5): ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            cycleTime = 1000 * (int.Parse(Console.ReadLine()));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\nPRESS <CTRL-C> TO END THE SIMULATION\n");
            Console.Write("\nPRESS <ENTER> TO START");
            Console.CursorVisible = false;
            runSimulation = true;
            Console.ReadLine();

            Status[,] grid = new Status[Rows, Columns];

            // randomly initialize our grid
            for (int row = 0; row < Rows; row++) {
                for (int column = 0; column < Columns; column++) {
                    grid[row, column] = (Status)rnd.Next(0, 2);
                }
            }

            Console.CancelKeyPress += (sender, args) =>
            {
                runSimulation = false;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n==> End of simulation.");
            };

            // let's give our console
            // a good scrubbing

            cycle = 0;

            // Displaying the grid 
            while (runSimulation) {
                Print(grid);
                grid = NextGeneration(grid);
            }
        }

        private static Status[,] NextGeneration(Status[,] currentGrid)
        {
            var nextGeneration = new Status[Rows, Columns];

            // Loop through every cell 
            for (int row = 1; row < Rows - 1; row++)
                for (int column = 1; column < Columns - 1; column++)
                {
                    // find your alive neighbors
                    int aliveNeighbors = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            aliveNeighbors += currentGrid[row + i, column + j] == Status.Alive ? 1 : 0;
                        }
                    }

                    var currentCell = currentGrid[row, column];

                    // The cell needs to be subtracted 
                    // from its neighbours as it was  
                    // counted before 
                    aliveNeighbors -= currentCell == Status.Alive ? 1 : 0;

                    // Implementing the Rules of Life 

                    // Cell is lonely and dies 
                    if (currentCell.HasFlag(Status.Alive) && aliveNeighbors < 2)
                    {
                        nextGeneration[row, column] = Status.Dead;
                    }

                    // Cell dies due to over population 
                    else if (currentCell.HasFlag(Status.Alive) && aliveNeighbors > 3)
                    {
                        nextGeneration[row, column] = Status.Dead;
                    }

                    // A new cell is born 
                    else if (currentCell.HasFlag(Status.Dead) && aliveNeighbors == 3)
                    {
                        nextGeneration[row, column] = Status.Alive;
                    }
                    // stays the same
                    else
                    {
                        nextGeneration[row, column] = currentCell;
                    }
                }
            return nextGeneration;
        }

        // TODO clean up the unused timeout variable -- replaced with cycletime
        private static void Print(Status[,] future, int timeout = 5000)
        {
            int stillBreathing = 0;

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Status cell = future[row, column];
                    Console.ForegroundColor = (cell == Status.Alive ? ConsoleColor.Green : ConsoleColor.DarkRed);
                    Console.Write(cell == Status.Alive ? "*" : ".");
                    if (cell == Status.Alive) stillBreathing++;
                }
                Console.Write("\n");
            }
            cycle++;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nResult of cycle:       {cycle}");
            Console.WriteLine($"\nNumber of cells alive: {stillBreathing}");

            Thread.Sleep(cycleTime);
        }
    }

    public enum Status
    {
        Dead,
        Alive
    }
}
