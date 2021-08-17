using System.Windows.Controls;
using GameOfLife;
using GameOfLifeDesktop.UILibrary; // 2.3

namespace GameOfLifeDesktop
{
    class MyMainWindow : GameOfLifeMainWindow // 2.3
    {        
        protected override void OnStartSimulation() // 3.1
        { // 3.1
            ToggleInputEnabled(false); // 3.2
            Game.Rows = int.Parse(numOfRowsTextBox.Text); // 3.2
            Game.Columns = int.Parse(numOfColTextBox.Text); // 3.2
            Game.CycleTime = 1000 * double.Parse(cycleTimeTextBox.Text); // 3.2

            // Create a new grid that will contain our game
            cycleGrid = new Grid(); // 3.3 
            SetCycleGridRowsAndColumns(); // 3.3

            // Populate and/or inflate user interface (UI)           
            for (int row = 0; row < Game.Rows; row++) // 3.4
            { // 3.4
                for (int column = 0; column < Game.Columns; column++) // 3.5
                { // 3.5
                    Image cell = new Image(); // 3.6
                    cell.SetCellRowAndColumn(row, column); // 3.6
                    cell.Source = SelectedInstructorImage; // 3.6
                    cycleGrid.Children.Add(cell); // 3.6
                } // 3.5
            } // 3.4

            // Add our grid to the user interface as a sub (child) grid of the maingrid
            mainGrid.Children.Add(cycleGrid); // 3.7
            Game.Start(); // 3.7
        } // 3.1

        protected override void OnStopSimulation()
        {
            Game.Stop();
        }

        protected override void OnResumeSimulation()
        {            
            Game.Resume();
        }

        protected override void OnResetSimulation()
        {
            // Create a new game
            Game = new GameOfLifeSession();
            // Remove our cycleGrid form the maingrid
            mainGrid.Children.Remove(cycleGrid);

            ClearUserInput();
            ToggleInputEnabled(true);
        }

        protected override void OnNextCycle(GameOfLifeSession game, Status[,] nextCycle)
        {
            for (int row = 0; row < Game.Rows; row++)
            {
                for (int column = 0; column < Game.Columns; column++)
                {
                    // row * totalColumns + column -- to get correct column based on grid's index
                    Status dataCell = nextCycle[row, column];
                    UpdateWindow(() =>
                    {
                        Image imgCell = GetCellByRowAndColumn(row, column);
                        if (dataCell == Status.Alive)
                            imgCell.Source = SelectedInstructorImage;
                        else
                            imgCell.Source = null;
                    });
                }
            }

            UpdateCycleStatistics();
        }
    }
}
