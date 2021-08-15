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
        protected override void OnSetupSimulation()
        {
            InstructorImages = new BitmapImage[IMAGE_FILES.Length];
            for (int i = 0; i < IMAGE_FILES.Length; i++)
                InstructorImages[i] = new BitmapImage(new Uri($"pack://application:,,,/GameOfLifeDesktop.UILibrary;component/Images/{IMAGE_FILES[i]}.jpg"));
            listView.ItemsSource = InstructorImages;
        }

        protected override void OnStartSimulation()
        {
            ToggleInputEnabled(false);
            game.Rows = int.Parse(numOfRowsTextBox.Text);
            game.Columns = int.Parse(numOfColTextBox.Text);
            game.CycleTime = 1000 * double.Parse(cycleTimeTextBox.Text);

            // Create a new grid that will contain our game
            cycleGrid = new Grid();

            // Setup row definitions
            for (int cols = 0; cols < game.Columns; cols++)
                cycleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Setup column definitions
            for (int rows = 0; rows < game.Rows; rows++)
                cycleGrid.RowDefinitions.Add(new RowDefinition());

            // Populate and/or inflate user interface (UI)           
            for (int rows = 0; rows < game.Rows; rows++)
                for (int columns = 0; columns < game.Columns; columns++)
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
            game.Start();
        }

        protected override void OnStopSimulation()
        {
            game.Stop();
        }

        protected override void OnResumeSimulation()
        {
            game.Resume();
        }

        protected override void OnResetSimulation()
        {
            // Create a new game
            game = new GameOfLifeSession();
            // Remove our cycleGrid form the maingrid
            mainGrid.Children.Remove(cycleGrid);

            ClearUserInput();
        }

        protected override void OnNextCycle(GameOfLifeSession game, Status[,] nextCycle)
        {
            for (int row = 0; row < this.game.Rows; row++)
            {
                for (int column = 0; column < this.game.Columns; column++)
                {
                    // row * totalColumns + column -- to get correct column based on grid's index
                    Status dataCell = nextCycle[row, column];
                    Dispatcher.Invoke(() =>
                    {
                        Image imgCell = (Image)cycleGrid.Children[row * this.game.Columns + column];
                        if (dataCell == Status.Alive)
                            imgCell.Source = SelectedInstructorImage;
                        else
                            imgCell.Source = null;
                    });
                }
            }

            Dispatcher.Invoke(() =>
            {
                cycleLabel.Content = this.game.CycleCounter;
                aliveLabel.Content = this.game.AliveCounter;
            });
        }
    }
}
