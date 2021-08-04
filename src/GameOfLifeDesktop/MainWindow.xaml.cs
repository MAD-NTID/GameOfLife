using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GameOfLife;

namespace GameOfLifeDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string START_BTN_TEXT = "Start";
        const string STOP_BTN_TEXT = "Stop";
        const string RESUME_BTN_TEXT = "Resume";

        public GameOfLifeSession Game { get; private set; } = new GameOfLifeSession();

        public BitmapImage AliveImg { get; private set; }

        private Grid cycleGrid;

        public MainWindow()
        {
            InitializeComponent();

            AliveImg = new BitmapImage(new System.Uri("pack://application:,,,/Images/dog.jpg"));
        }

        private void OnStartBtn_Clicked(object sender, RoutedEventArgs e)
        {
            toggleBtn.Click -= OnStartBtn_Clicked; // Detach current toggle, start
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
                    cell.Source = AliveImg;
                    // Set row/column for each label here
                    cell.SetValue(Grid.RowProperty, rows);
                    cell.SetValue(Grid.ColumnProperty, columns);
                    // Add label to grid                            
                    cycleGrid.Children.Add(cell);
                }
                         
            // Add our grid to the user interface as a sub (child) grid of the maingrid
            mainGrid.Children.Add(cycleGrid);

            // Be notified when the next cycle is generate by our GameOfLive session
            Game.NextCycle += Game_NextCycle;
            Game.Start();
            toggleBtn.Click += OnStopBtn_Clicked; // Attack next toggle, stop
            toggleBtn.Content = STOP_BTN_TEXT;     
        }

        private void OnStopBtn_Clicked(object sender, RoutedEventArgs e)
        {
            toggleBtn.Click -= OnStopBtn_Clicked;
            Game.Stop();
            // Reseting is only possible when the simulation has stopped
            resetBtn.IsEnabled = true;

            toggleBtn.Click += OnResumeBtn_Clicked;
            toggleBtn.Content = RESUME_BTN_TEXT;
        }

        private void OnResumeBtn_Clicked(object sender, RoutedEventArgs e)
        {
            resetBtn.IsEnabled = false; // Resuming so disable the reset button
            toggleBtn.Click -= OnResumeBtn_Clicked;
            toggleBtn.Click += OnStopBtn_Clicked;
            toggleBtn.Content = STOP_BTN_TEXT;
            Game.Resume();
        }

        private void OnResetBtn_Clicked(object sender, RoutedEventArgs e)
        {
            // Apply needed button changes for a reset
            toggleBtn.Click -= OnResumeBtn_Clicked;
            toggleBtn.Click += OnStartBtn_Clicked;
            toggleBtn.Content = START_BTN_TEXT;

            // Create a new game
            Game = new GameOfLifeSession();
            // Remove our cycleGrid form the maingrid
            mainGrid.Children.Remove(cycleGrid);

            // Reset all the input boxes
            numOfColTextBox.Text = string.Empty;
            numOfRowsTextBox.Text = string.Empty;
            cycleTimeTextBox.Text = string.Empty;

            // Reset game status
            cycleLabel.Content = string.Empty;
            aliveLabel.Content = string.Empty;
        }

        private void Game_NextCycle(object sender, NextCycleEventArgs e)
        {           
            for (int row = 0; row < Game.Rows; row++)            
                for (int column = 0; column < Game.Columns; column++)
                {
                    // row * totalColumns + column -- to get correct column based on grid's index
                    Status dataCell = e.NextCycle[row, column];
                    Dispatcher.Invoke(() =>
                    {
                        Image imgCell = (Image)cycleGrid.Children[row * Game.Columns + column];
                        if (dataCell == Status.Alive)
                            imgCell.Source = AliveImg;
                        else
                            imgCell.Source = null;
                    });
                }
            
            Dispatcher.Invoke(() =>
            {
                cycleLabel.Content = Game.CycleCounter;
                aliveLabel.Content = Game.AliveCounter;
            });            
        }

        private void ResetBtnClicked(object sender, RoutedEventArgs e)
            => Game = new GameOfLifeSession();        
    }
}
