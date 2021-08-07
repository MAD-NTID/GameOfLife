using GameOfLife;
using System;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace GameOfLifeMobile
{
    public partial class MainPage : ContentPage
    {
        private readonly string[] IMAGE_FILES = new string[] {
            "mark_reynolds",
            "brian_trager",
            "walter_bubie",
            "james_mallory",
            "edmund_lucas",
            "joseph_stanislow",
            "mark_jeremy",
            "brian_nadworny",
            "thomas_simpson",
            "triandre_turner",
            "tao_end"
        };

        const string START_BTN_TEXT = "Start";
        const string STOP_BTN_TEXT = "Stop";
        const string RESUME_BTN_TEXT = "Resume";

        public GameOfLifeSession Game { get; private set; } = new GameOfLifeSession();

        public ImageSource[] InstructorImages { get; private set; } 

        private Grid cycleGrid;

        public ImageSource SelectedImage { get; set; }

        public MainPage()
        {
            InitializeComponent();
            InstructorImages = new ImageSource[IMAGE_FILES.Length];
            for (int i = 0; i < IMAGE_FILES.Length; i++)            
                InstructorImages[i] = ImageSource.FromFile((Device.RuntimePlatform == Device.Android ? IMAGE_FILES[i] : $"Images/{IMAGE_FILES[i]}") + ".jpg");
            collectionView.ItemsSource = InstructorImages;
        }

        protected override void OnAppearing()
        {
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            DeviceDisplay.MainDisplayInfoChanged -= DeviceDisplay_MainDisplayInfoChanged;
            base.OnDisappearing();
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            if (e.DisplayInfo.Orientation == DisplayOrientation.Portrait) // Portrait
            {
                mainLayout.Orientation = StackOrientation.Vertical;
            }
            else // Landscape
            {
                mainLayout.Orientation = StackOrientation.Horizontal;
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
            {
                mainGrid.WidthRequest = width;
                mainGrid.HeightRequest = width;
            }            
            else
            {
                mainGrid.WidthRequest = height;
                mainGrid.HeightRequest = height;
            }
            base.OnSizeAllocated(width, height);
        }

        private void OnStartBtn_Clicked(object sender, EventArgs e)
        {
            toggleBtn.Clicked -= OnStartBtn_Clicked; // Detach current toggle, start

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
                    cell.Source = SelectedImage;                        
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
            // Disable once clicked
            resetBtn.IsEnabled = false;

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

            ToggleInputEnabled(true);
        }

        private void OnResetBtnClicked(object sender, ClickedEventArgs e)
            => Game = new GameOfLifeSession();

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
                            imgCell.Source = SelectedImage;
                        else
                            imgCell.Source = null;
                    });
                }

            Device.BeginInvokeOnMainThread(() =>
            {
                cycleLabel.Text = Game.CycleCounter.ToString();
                aliveLabel.Text = Game.AliveCounter.ToString();
            });
        }        

        private void ToggleInputEnabled(bool value)
        {
            numOfRowsTextBox.IsEnabled = value;
            numOfColTextBox.IsEnabled = value;
            cycleTimeTextBox.IsEnabled = value;
        }        

        private void OnCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedImage = (ImageSource)e.CurrentSelection[0];
        }       
    }
}
