using System;
using System.Timers;

namespace GameOfLife
{
    public class GameOfLifeSession
    {
        public const int DEFAULT_CYCLE_TIME = 2000;

        public event EventHandler<GameUpdateEventArgs> NextCycle;

        #region Properties
        public int Rows { get; set; }
        public int Columns { get; set; }
        public bool IsRunning
        {
            get => timer.Enabled;
            private set => timer.Enabled = value;
        }

        public double CycleTime
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }
        public int Cycle { get; private set; }
        public int CurrentlyAlive { get; private set; }
        #endregion Properties

        #region Fields
        private static readonly Random rnd = new Random();
        private Status[,] currentCycle;
        private readonly Timer timer = new Timer(DEFAULT_CYCLE_TIME);
        #endregion

        public void Start()
        {
            if (IsRunning)
                return;
            
            timer.Elapsed += Timer_Elapsed;

            currentCycle = new Status[Rows, Columns];

            // randomly initialize our grid
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    currentCycle[row, column] = (Status)rnd.Next(0, 2);
                }
            }

            IsRunning = true;
            NextCycle?.Invoke(this, new GameUpdateEventArgs(currentCycle));            
        }        

        public void Stop()
            => IsRunning = false;        

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
            => OnNextCycle();        

        protected virtual void OnNextCycle()
        {
            Cycle++;
            var nextCycle = new Status[Rows, Columns];
            CurrentlyAlive = 0;

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
                            aliveNeighbors += currentCycle[row + i, column + j] == Status.Alive ? 1 : 0;
                        }
                    }

                    var currentCell = currentCycle[row, column];

                    if (currentCell == Status.Alive) CurrentlyAlive++;

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
                }
            currentCycle = nextCycle;
            NextCycle?.Invoke(this, new GameUpdateEventArgs(nextCycle));
        }
    }

    public enum Status
    {
        Dead,
        Alive
    }

    public class GameUpdateEventArgs : EventArgs
    {
        public Status[,] NextCycle { get; set; }

        public GameUpdateEventArgs(Status[,] grid)
            => NextCycle = grid;
    }
}