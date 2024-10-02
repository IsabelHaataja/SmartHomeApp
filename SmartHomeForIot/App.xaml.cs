using SmartHomeForIot.Pages;
using SmartHomeForIot.ViewModels;

namespace SmartHomeForIot
{
    public partial class App : Application
    {
        //private readonly OServiceProvider _serviceProvider;
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // var mainPage = _serviceProvider.GetRequiredService<MainPage>();
            // MainPage = new NavigationPage(new MainPage());
        }

    }
}
