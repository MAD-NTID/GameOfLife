using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls; // 3.3
using GameOfLife; // 4.1
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

            cycleGrid = new Grid(); // 3.3 
            SetCycleGridRowsAndColumns(); // 3.3

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

            mainGrid.Children.Add(cycleGrid); // 3.7
            Game.Start(); // 3.7
        } // 3.1        

        protected override void OnNextCycle(Status[,] nextCycle) // 4.1
        { // 4.1
            for (int row = 0; row < Game.Rows; row++) // 4.2
            { // 4.2
                for (int column = 0; column < Game.Columns; column++) // 4.3
                { // 4.3
                    Status dataCell = nextCycle[row, column]; // 4.4
                    UpdateWindow(() => // 4.5
                    { // 4.5
                        Image imgCell = GetCellByRowAndColumn(row, column); // 4.6
                        if (dataCell == Status.Alive) // 4.6
                            imgCell.Source = SelectedInstructorImage; // 4.6
                        else // 4.6
                            imgCell.Source = null;  // 4.6
                    }); // 4.5
                } // 4.3
            } // 4.2

            UpdateCycleStatistics(); // 4.7
        } // 4.1

        protected override void OnStopSimulation() // 5.1
        { // 5.1
            Game.Stop(); // 5.2
        } // 5.1

        protected override void OnResumeSimulation() // 6.1
        { // 6.1
            Game.Resume(); // 6.2
        } // 6.1

        protected override void OnResetSimulation() // 7.1
        { // 7.1
            Game = new GameOfLifeSession(); // 7.2
            mainGrid.Children.Remove(cycleGrid); // 7.2

            ClearUserInput(); // 7.3
            ToggleInputEnabled(true); // 7.3
        } // 7.1
    }
}
