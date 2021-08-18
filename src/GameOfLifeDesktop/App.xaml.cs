using System.Windows;

namespace GameOfLifeDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {             
            Current.MainWindow = new MyMainWindow();
            Current.MainWindow.Show();
        }
    }
}
