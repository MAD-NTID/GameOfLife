using GameOfLife;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GameOfLifeMobile
{
    public partial class MainPage : ContentPage
    {
        const string START_BTN_TEXT = "Start";
        const string STOP_BTN_TEXT = "Stop";
        const string RESUME_BTN_TEXT = "Resume";

        public GameOfLifeSession Game { get; private set; } = new GameOfLifeSession();

        public ImageSource AliveImg { get; private set; }

        private Grid cycleGrid;

        public MainPage()
        {
            InitializeComponent();
            mainGrid.WidthRequest = Width;
            mainGrid.HeightRequest = Width;
            AliveImg = ImageSource.FromFile(Device.RuntimePlatform == Device.Android ? "dog.jpg" : "Images/dog.jpg");
        }

        private void OnStartBtn_Clicked(object sender, EventArgs e)
        {
            toggleBtn.Clicked -= OnStartBtn_Clicked; // Detach current toggle, start
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
            {
                for (int rows = 0; rows < Game.Rows; rows++)
                {
                    //cycleGrid.RowDefinitions.Add(new RowDefinition());
                    for (int columns = 0; columns < Game.Columns; columns++)
                    {
                        //cycleGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        Image cell = new Image();
                        cell.Source = AliveImg;                        
                        // Set row/column for each label here
                        cell.SetValue(Grid.RowProperty, rows);
                        cell.SetValue(Grid.ColumnProperty, columns);
                        // Add label to grid                            
                        cycleGrid.Children.Add(cell);
                    }
                }
            }

            // Add our grid to the user interface as a sub (child) grid of the maingrid
            mainGrid.Children.Add(cycleGrid);

            // Be notified when the next cycle is generate by our GameOfLive session
            Game.NextCycle += Game_NextCycle;
            Game.Start();
            toggleBtn.Clicked += OnStopBtn_Clicked; // Attack next toggle, stop
            toggleBtn.Text = STOP_BTN_TEXT;
        }

        private void OnStopBtn_Clicked(object sender, EventArgs e)
        {
            toggleBtn.Clicked -= OnStopBtn_Clicked;
            Game.Stop();
            // Reseting is only possible when the simulation has stopped
            resetBtn.IsEnabled = true;

            toggleBtn.Clicked += OnResumeBtn_Clicked;
            toggleBtn.Text = RESUME_BTN_TEXT;
        }

        private void OnResumeBtn_Clicked(object sender, EventArgs e)
        {
            resetBtn.IsEnabled = false; // Resuming so disable the reset button
            toggleBtn.Clicked -= OnResumeBtn_Clicked;
            toggleBtn.Clicked += OnStopBtn_Clicked;
            toggleBtn.Text = STOP_BTN_TEXT;
            Game.Resume();
        }

        private void OnResetBtn_Clicked(object sender, EventArgs e)
        {
            // Apply needed button changes for a reset
            toggleBtn.Clicked -= OnResumeBtn_Clicked;
            toggleBtn.Clicked += OnStartBtn_Clicked;
            toggleBtn.Text = START_BTN_TEXT;

            // Create a new game
            Game = new GameOfLifeSession();
            // Remove our cycleGrid form the maingrid
            mainGrid.Children.Remove(cycleGrid);

            // Reset all the input boxes
            numOfColTextBox.Text = string.Empty;
            numOfRowsTextBox.Text = string.Empty;
            cycleTimeTextBox.Text = string.Empty;

            // Reset game status
            cycleLabel.Text = string.Empty;
            aliveLabel.Text = string.Empty;
        }

        private void Game_NextCycle(object sender, NextCycleEventArgs e)
        {
            for (int row = 0; row < Game.Rows; row++)
                for (int column = 0; column < Game.Columns; column++)
                {
                    // row * totalColumns + column -- to get correct column based on grid's index
                    Status dataCell = e.NextCycle[row, column];
                    int cpyRow = new int();
                    int cpyCol = new int();
                    cpyRow = row;
                    cpyCol = column;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Image imgCell = (Image)cycleGrid.Children[cpyRow * Game.Columns + cpyCol];
                        if (dataCell == Status.Alive)
                            imgCell.Source = AliveImg;
                        else
                            imgCell.Source = null;
                    });
                }

            //for (int row = 0; row < Game.Rows; row++)            
            //    for (int column = 0; column < Game.Columns; column++)
            //    {
            //        // row * totalColumns + column -- to get correct column based on grid's index
            //        Status dataCell = e.NextCycle[row, column];
            //        Device.BeginInvokeOnMainThread(() =>
            //        {                        
            //            if (row == 5 && column == 5)
            //                Console.WriteLine();
            //            Image imgCell = (Image)cycleGrid.Children[row * Game.Columns + column];
            //            if (dataCell == Status.Alive)
            //                imgCell.Source = AliveImg;
            //            else
            //                imgCell.Source = null;
            //        });
            //    }

            Device.BeginInvokeOnMainThread(() =>
            {
                cycleLabel.Text = Game.CycleCounter.ToString();
                aliveLabel.Text = Game.AliveCounter.ToString();
            });
        }

        private void ResetBtnClicked(object sender, ClickedEventArgs e)
            => Game = new GameOfLifeSession();
    }
}
