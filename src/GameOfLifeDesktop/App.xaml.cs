using System.Windows;

namespace GameOfLifeDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() // 2.4
        { // 2.4
            Current.MainWindow = new MyMainWindow(); // 2.4
            Current.MainWindow.Show(); // 2.4
        } // 2.4
    }
}
