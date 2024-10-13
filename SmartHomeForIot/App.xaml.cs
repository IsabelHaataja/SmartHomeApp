using SmartHomeForIot.Pages;
using SmartHomeForIot.ViewModels;
using System.Diagnostics;

namespace SmartHomeForIot
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
