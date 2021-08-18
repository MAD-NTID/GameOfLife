using System;
using System.Collections.Generic;
using System.Text;
using GameOfLife;
using GameOfLifeMobile.UILibrary;
using Xamarin.Forms;

namespace GameOfLifeMobile
{
    public class GameOfLifeView : GameOfLifeMainPage
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
            {
                for (int column = 0; column < Game.Columns; column++)
                {
                    Image cell = new Image();
                    cell.Source = SelectedInstructorImage;
                    cell.SetCellRowAndColumn(row, column);
                    // Add label to grid                            
                    cycleGrid.Children.Add(cell);
                }
            }

            // Add our grid to the user interface as a sub (child) grid of the maingrid
            mainGrid.Children.Add(cycleGrid);
            // Start the game
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

        protected override void OnNextCycle(Status[,] nextCycle)
        {
            for (int row = 0; row < Game.Rows; row++)
            {
                for (int column = 0; column < Game.Columns; column++)
                {
                    // row * totalColumns + column -- to get correct column based on grid's index
                    Status dataCell = nextCycle[row, column];
                    int cpyRow = new int();
                    int cpyCol = new int();
                    cpyRow = row;
                    cpyCol = column;
                    UpdateWindow(() =>
                    {
                        Image imgCell = (Image)cycleGrid.Children[cpyRow * Game.Columns + cpyCol];
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
