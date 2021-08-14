using System;
using System.Timers;

namespace GameOfLife
{
    /// <summary>
    /// A <see cref="GameOfLifeSession"/> is a class that contains all the backing functionality / states needed support a game regardless of how it is presented.
    /// </summary>
    public class GameOfLifeSession
    {
        /// <summary>
        /// Default internval for cycles in milliseconds.
        /// </summary>
        public const int DEFAULT_CYCLE_TIME = 2000;

        /// <summary>
        /// Notifies subscribers that the next cycle is ready.
        /// </summary>
        public event EventHandler<NextCycleEventArgs> NextCycle;

        #region Properties
        /// <summary>
        /// Rows of the GameOfLife's grid.
        /// </summary>
        public int Rows { get; set; }
        /// <summary>
        /// Columns of the GameOfLife's grid.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Gets the running status of the game.
        /// </summary>
        public bool IsRunning
        {
            get => timer.Enabled;
            private set => timer.Enabled = value;
        }

        /// <summary>
        /// Gets or sets the amount of time between cycles.
        /// </summary>
        public double CycleTime
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        /// <summary>
        /// Counter the total number of cycles, whereas the last cycle is the current.
        /// </summary>
        public int CycleCounter { get; private set; }
        /// <summary>
        /// Counts the number of alive cells in the current cycle.
        /// </summary>
        public int AliveCounter { get; private set; }
        #endregion Properties

        #region Fields
        private static readonly Random rnd = new Random();
        private Status[,] currentCycle;
        private readonly Timer timer = new Timer(DEFAULT_CYCLE_TIME);
        #endregion

        /// <summary>
        /// Start the game by setting everything up and then starting the timer.
        /// </summary>
        public void Start()
        {
            if (IsRunning)
                return;
            
            timer.Elapsed += Timer_Elapsed;

            CycleCounter = 0;
            AliveCounter = 0;
            currentCycle = new Status[Rows, Columns];

            // randomly initialize our grid
            for (int row = 0; row < Rows; row++)            
                for (int column = 0; column < Columns; column++)
                {
                    currentCycle[row, column] = (Status)rnd.Next(0, 2);
                    if (currentCycle[row, column] == Status.Alive)
                        AliveCounter++;
                }                       

            IsRunning = true;
            NextCycle?.Invoke(this, new NextCycleEventArgs(currentCycle));            
        }        

        /// <summary>
        /// Stop the game via the underlying timer.
        /// </summary>
        public void Stop()
            => IsRunning = false;

        /// <summary>
        /// Restart the game via the underlying timer.
        /// </summary>
        public void Resume()
            => IsRunning = true;

        /// <summary>
        /// Occurs when the timer elapses based off the interval.
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
            => OnNextCycle();        

        /// <summary>
        /// Prepares everything needed for the next cycle and then invokes <see cref="NextCycle"/>.
        /// </summary>
        protected virtual void OnNextCycle()
        {
            CycleCounter++;
            var nextCycle = new Status[Rows, Columns];
            AliveCounter = 0;

            // Loop through every cell 
            for (int row = 1; row < Rows - 1; row++)
                for (int column = 1; column < Columns - 1; column++)
                {
                    // find your alive neighbors
                    int aliveNeighbors = 0;
                    for (int i = -1; i <= 1; i++)                    
                        for (int j = -1; j <= 1; j++)                        
                            aliveNeighbors += currentCycle[row + i, column + j] == Status.Alive ? 1 : 0;                    

                    var currentCell = currentCycle[row, column];                    

                    // The cell needs to be subtracted 
                    // from its neighbours as it was  
                    // counted before 
                    aliveNeighbors -= currentCell == Status.Alive ? 1 : 0;

                    // Implementing the Rules of Life 

                    switch (currentCell)
                    {
                        case Status.Alive when aliveNeighbors < 2: // To few connected cells, so die
                            nextCycle[row, column] = Status.Dead;
                            break;
                        case Status.Alive when aliveNeighbors > 3: // To many connected cells, so die
                            nextCycle[row, column] = Status.Dead;
                            break;
                        case Status.Dead when aliveNeighbors == 3: // Enough connected cells, so revive
                            nextCycle[row, column] = Status.Alive;
                            break;
                        default: // Carry on
                            nextCycle[row, column] = currentCell;
                            break;
                    }

                    if (nextCycle[row, column] == Status.Alive) 
                        AliveCounter++;
                }
            currentCycle = nextCycle;
            NextCycle?.Invoke(this, new NextCycleEventArgs(nextCycle));
        }
    }

    /// <summary>
    /// Status of a cell.
    /// </summary>
    public enum Status
    {
        Dead,
        Alive
    }

    /// <summary>
    /// Contains information about the next cycle.
    /// </summary>
    public class NextCycleEventArgs : EventArgs
    {
        public Status[,] NextCycle { get; set; }

        public NextCycleEventArgs(Status[,] grid)
            => NextCycle = grid;
    }
}