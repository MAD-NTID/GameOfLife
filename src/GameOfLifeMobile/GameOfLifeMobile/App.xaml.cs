using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameOfLifeMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new GameOfLifeView();
        }
    }
}
