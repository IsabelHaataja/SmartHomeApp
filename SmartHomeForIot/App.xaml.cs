using SmartHomeForIot.Pages;
using SmartHomeForIot.ViewModels;

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
