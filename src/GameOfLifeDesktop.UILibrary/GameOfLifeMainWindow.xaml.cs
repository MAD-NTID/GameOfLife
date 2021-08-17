using GameOfLife;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.IO.Packaging;
using System.Windows.Navigation;
using System.Reflection;
using System.Windows.Markup;

namespace GameOfLifeDesktop.UILibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameOfLifeMainWindow : Window
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

        public BitmapImage[] InstructorImages { get; protected set; }
        public BitmapImage SelectedInstructorImage { get; protected set; }

        protected Grid cycleGrid;

        private bool hasDetachedResetHandlers;

        public GameOfLifeMainWindow()
        {
            // Custom InitializeComponent
            this.LoadViewFromUri("/GameOfLifeDesktop.UILibrary;component/GameOfLifeMainWindow.xaml");
            InstructorImages = new BitmapImage[IMAGE_FILES.Length];
            for (int i = 0; i < IMAGE_FILES.Length; i++)
                InstructorImages[i] = new BitmapImage(new Uri($"pack://application:,,,/GameOfLifeDesktop.UILibrary;component/Images/{IMAGE_FILES[i]}.jpg"));
            listView.ItemsSource = InstructorImages;
        }

        #region Student Implementation Methods
        protected virtual void OnStartSimulation() { }
        protected virtual void OnResumeSimulation() { }
        protected virtual void OnStopSimulation() { }
        protected virtual void OnResetSimulation() { }

        protected virtual void OnNextCycle(GameOfLifeSession game, Status[,] nextCycle) { }
        #endregion

        private void OnStartBtn_Clicked(object sender, RoutedEventArgs e)
        {
            hasDetachedResetHandlers = false;
            // Be notified when the next cycle is generate by our GameOfLive session
            Game.NextCycle += Game_NextCycle;
            toggleBtn.Click -= OnStartBtn_Clicked; // Detach current toggle, start
            resetBtn.IsEnabled = false;

            OnStartSimulation(); // <- Student code goes in overridden

            toggleBtn.Click += OnStopBtn_Clicked; // Attack next toggle, stop
            toggleBtn.Content = STOP_BTN_TEXT;
        }

        private void OnResumeBtn_Clicked(object sender, RoutedEventArgs e)
        {
            resetBtn.IsEnabled = false; // Resuming so disable the reset button
            toggleBtn.Click -= OnResumeBtn_Clicked;
            toggleBtn.Click += OnStopBtn_Clicked;
            toggleBtn.Content = STOP_BTN_TEXT;

            OnResumeSimulation();
        }

        private void OnStopBtn_Clicked(object sender, RoutedEventArgs e)
        {
            toggleBtn.Click -= OnStopBtn_Clicked;

            OnStopSimulation(); // <- Student code goes in overridden

            // Reseting is only possible when the simulation has stopped
            resetBtn.IsEnabled = true;

            toggleBtn.Click += OnResumeBtn_Clicked;
            toggleBtn.Content = RESUME_BTN_TEXT;
        }

        private void OnResetBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (!hasDetachedResetHandlers)
            {
                // Apply needed button changes for a reset
                toggleBtn.Click -= OnResumeBtn_Clicked;
                toggleBtn.Click += OnStartBtn_Clicked;
                toggleBtn.Content = START_BTN_TEXT;
                hasDetachedResetHandlers = true;
            }            

            OnResetSimulation();
        }

        private void Game_NextCycle(GameOfLifeSession game, Status[,] nextCycle)
            => OnNextCycle(game, nextCycle);

        protected void ToggleInputEnabled(bool value)
        {
            Dispatcher.Invoke(() =>
            {
                numOfRowsTextBox.IsEnabled = value;
                numOfColTextBox.IsEnabled = value;
                cycleTimeTextBox.IsEnabled = value;
            });
        }

        protected void ClearUserInput()
        {
            Dispatcher.Invoke(() =>
            {
                // Reset all the input boxes
                numOfColTextBox.Text = string.Empty;
                numOfRowsTextBox.Text = string.Empty;
                cycleTimeTextBox.Text = string.Empty;

                // Reset game status
                cycleLabel.Content = string.Empty;
                aliveLabel.Content = string.Empty;
            });
        }

        private void OnListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedInstructorImage = (BitmapImage)((ListView)sender).SelectedItem;
            e.Handled = true;
        }
    }

    static class Extension
    {
        /// <summary>
        /// I did not come up with this function, I don't have time to study the reflection taking place to load the .xaml files, hence: 
        /// https://stackoverflow.com/questions/7646331/the-component-does-not-have-a-resource-identified-by-the-uri was used.
        /// </summary>
        public static void LoadViewFromUri(this GameOfLifeMainWindow userControl, string baseUri)
        {
            try
            {
                var resourceLocater = new Uri(baseUri, UriKind.Relative);
                var exprCa = (PackagePart)typeof(Application).GetMethod("GetResourceOrContentPart", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { resourceLocater });
                var stream = exprCa.GetStream();
                var uri = new Uri((Uri)typeof(BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null), resourceLocater);
                var parserContext = new ParserContext
                {
                    BaseUri = uri
                };
                typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { stream, parserContext, userControl, true });
            }
            catch (Exception)
            {
                //log
            }
        }
    }
}
