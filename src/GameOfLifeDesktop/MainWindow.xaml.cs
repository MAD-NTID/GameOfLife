using System.Windows;
using System.Windows.Controls;
using GameOfLife;

namespace GameOfLifeDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameOfLifeSession Game { get; private set; } = new GameOfLifeSession();

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void ToggleBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (Game.IsRunning) // Stop if running
                Game.Stop();
            else // Start if not running
            {
                Game.Rows = int.Parse(numOfRowsTextBox.Text);
                Game.Columns = int.Parse(numOfColTextBox.Text);
                Game.CycleTime = 1000 * double.Parse(cycleTimeTextBox.Text);

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
                            Label l = new Label
                            {
                                Content = $"?"
                            };

                            // Set row/column for each label here
                            l.SetValue(Grid.RowProperty, rows);
                            l.SetValue(Grid.ColumnProperty, columns);
                            // Add label to grid                            
                            int index = cycleGrid.Children.Add(l);
                            l.Content = index;
                        }
                    }
                }

                // Be notified when the next cycle is generate by our GameOfLive session
                Game.NextCycle += Game_NextCycle;
                Game.Start();
            }
        }

        private void Game_NextCycle(object sender, GameUpdateEventArgs e)
        {           
            for (int row = 0; row < Game.Rows; row++)
            {
                for (int column = 0; column < Game.Columns; column++)
                {
                    // row * totalColumns + column -- to get correct column based on grid's index
                    Status cell = e.NextCycle[row, column];
                    Dispatcher.Invoke(() =>
                    {
                        ((Label)cycleGrid.Children[row * Game.Columns + column]).Content = cell == Status.Alive ? "*" : ".";
                    });                    
                }
            }
            Dispatcher.Invoke(() =>
            {
                cycleLabel.Content = Game.Cycle;
                aliveLabel.Content = Game.CurrentlyAlive;
            });            
        }

        private void ResetBtnClicked(object sender, RoutedEventArgs e)
            => Game = new GameOfLifeSession();        
    }
}
