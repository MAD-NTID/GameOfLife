using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameOfLife;
using GameOfLifeDesktop.UILibrary;

namespace GameOfLifeDesktop
{
    public class MyMainWindow : GameOfLifeMainWindow
    {        
        protected override void OnStartSimulation()
        {
            ToggleInputEnabled(false);
            Game.Rows = int.Parse(numOfRowsTextBox.Text);
            Game.Columns = int.Parse(numOfColTextBox.Text);
            Game.CycleTime = 1000 * double.Parse(cycleTimeTextBox.Text);

            // Create a new grid that will contain our game
            cycleGrid = new Grid();
            SetCycleGridRowsAndColumns();

            // Populate and/or inflate user interface (UI)           
            for (int row = 0; row < Game.Rows; row++)
                for (int column = 0; column < Game.Columns; column++)
                {
                    Image cell = new Image();
                    cell.SetCellRowAndColumn(row, column);
                    cell.Source = SelectedInstructorImage;                          
                    cycleGrid.Children.Add(cell);
                }

            // Add our grid to the user interface as a sub (child) grid of the maingrid
            mainGrid.Children.Add(cycleGrid);
            Game.Start();
        }

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
                    Dispatcher.Invoke(() =>
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
