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

            // Setup row definitions
            for (int cols = 0; cols < Game.Columns; cols++)
                cycleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Setup column definitions
            for (int rows = 0; rows < Game.Rows; rows++)
                cycleGrid.RowDefinitions.Add(new RowDefinition());

            // Populate and/or inflate user interface (UI)           
            for (int rows = 0; rows < Game.Rows; rows++)
                for (int columns = 0; columns < Game.Columns; columns++)
                {
                    Image cell = new Image();
                    cell.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                    cell.Source = SelectedInstructorImage;
                    // Set row/column for each label here
                    cell.SetValue(Grid.RowProperty, rows);
                    cell.SetValue(Grid.ColumnProperty, columns);
                    // Add label to grid                            
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
            for (int row = 0; row < this.Game.Rows; row++)
            {
                for (int column = 0; column < this.Game.Columns; column++)
                {
                    // row * totalColumns + column -- to get correct column based on grid's index
                    Status dataCell = nextCycle[row, column];
                    Dispatcher.Invoke(() =>
                    {
                        Image imgCell = (Image)cycleGrid.Children[row * this.Game.Columns + column];
                        if (dataCell == Status.Alive)
                            imgCell.Source = SelectedInstructorImage;
                        else
                            imgCell.Source = null;
                    });
                }
            }

            Dispatcher.Invoke(() =>
            {
                cycleLabel.Content = this.Game.CycleCounter;
                aliveLabel.Content = this.Game.AliveCounter;
            });
        }
    }
}
