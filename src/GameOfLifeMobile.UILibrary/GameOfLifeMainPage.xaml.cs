using GameOfLife;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameOfLifeMobile.UILibrary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameOfLifeMainPage : ContentPage
    {
        protected readonly string[] IMAGE_FILES = new string[] {
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
            "tao_eng"
        };

        protected const string START_BTN_TEXT = "Start";
        protected const string STOP_BTN_TEXT = "Stop";
        protected const string RESUME_BTN_TEXT = "Resume";

        private GameOfLifeSession game = new GameOfLifeSession();
        protected GameOfLifeSession Game
        {
            get => game;
            set
            {
                if (game != null) // detach when new created to prevent memory leak / odd behavior
                    game.NextCycle -= Game_NextCycle;
                game = value;
            }
        }

        public ImageSource[] InstructorImages { get; protected set; }

        protected Grid cycleGrid;

        public ImageSource SelectedInstructorImage { get; protected set; }

        private bool hasDetachedResetHandlers;

        public GameOfLifeMainPage()
        {
            InitializeComponent();
            InstructorImages = new ImageSource[IMAGE_FILES.Length];
            for (int i = 0; i < IMAGE_FILES.Length; i++)
                InstructorImages[i] = ImageSource.FromFile(IMAGE_FILES[i] + ".jpg");
            collectionView.ItemsSource = InstructorImages;
        }

        #region Student Implementation Methods
        protected virtual void OnStartSimulation() { }
        protected virtual void OnResumeSimulation() { }
        protected virtual void OnStopSimulation() { }
        protected virtual void OnResetSimulation() { }

        protected virtual void OnNextCycle(Status[,] nextCycle) { }
        #endregion

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
                mainLayout.Orientation = StackOrientation.Vertical;
            else // Landscape            
                mainLayout.Orientation = StackOrientation.Horizontal;
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
                mainGrid.WidthRequest = width / 2;
                mainGrid.HeightRequest = height;
            }
            base.OnSizeAllocated(width, height);
        }

        private void OnStartBtn_Clicked(object sender, EventArgs e)
        {
            hasDetachedResetHandlers = false;
            resetBtn.IsEnabled = false;
            toggleBtn.Clicked -= OnStartBtn_Clicked; // Detach current toggle, start

            OnStartSimulation();

            // Be notified when the next cycle is generate by our GameOfLive session
            game.NextCycle += Game_NextCycle;
            toggleBtn.Clicked += OnStopBtn_Clicked; // Attack next toggle, stop
            toggleBtn.Text = STOP_BTN_TEXT;
        }

        private void OnStopBtn_Clicked(object sender, EventArgs e)
        {
            toggleBtn.Clicked -= OnStopBtn_Clicked;
            OnStopSimulation();
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
            OnResumeSimulation();
        }

        private void OnResetBtn_Clicked(object sender, EventArgs e)
        {
            if (!hasDetachedResetHandlers)
            {
                // Apply needed button changes for a reset
                toggleBtn.Clicked -= OnResumeBtn_Clicked;
                toggleBtn.Clicked += OnStartBtn_Clicked;
                toggleBtn.Text = START_BTN_TEXT;
                hasDetachedResetHandlers = true;
            }

            OnResetSimulation();
        }

        private void OnResetBtnClicked(object sender, ClickedEventArgs e)
            => game = new GameOfLifeSession();

        private void Game_NextCycle(GameOfLifeSession game, Status[,] nextCycle)
            => OnNextCycle(nextCycle);

        protected void ClearUserInput()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Reset all the input boxes
                numOfColTextBox.Text = string.Empty;
                numOfRowsTextBox.Text = string.Empty;
                cycleTimeTextBox.Text = string.Empty;

                // Reset game status
                cycleLabel.Text = string.Empty;
                aliveLabel.Text = string.Empty;
            });
        }

        protected void ToggleInputEnabled(bool value)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                numOfRowsTextBox.IsEnabled = value;
                numOfColTextBox.IsEnabled = value;
                cycleTimeTextBox.IsEnabled = value;
            });
        }

        protected void SetCycleGridRowsAndColumns()
        {
            // Setup row definitions
            for (int cols = 0; cols < Game.Columns; cols++)
                cycleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Setup column definitions
            for (int rows = 0; rows < Game.Rows; rows++)
                cycleGrid.RowDefinitions.Add(new RowDefinition());
        }

        protected Image GetCellByRowAndColumn(int row, int column)
            => (Image)cycleGrid.Children[row * Game.Columns + column];

        protected void UpdateCycleStatistics()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                cycleLabel.Text = Game.CycleCounter.ToString();
                aliveLabel.Text = Game.AliveCounter.ToString();
            });
        }

        protected void UpdateWindow(Action update)
            => Device.BeginInvokeOnMainThread(update);

        private void OnCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedInstructorImage = (ImageSource)e.CurrentSelection[0];
        }
    }

    public static class Extensions
    {
        public static void SetCellRowAndColumn(this Image img, int row, int column)
        {
            img.SetValue(Grid.RowProperty, row);
            img.SetValue(Grid.ColumnProperty, column);
        }
    }
}