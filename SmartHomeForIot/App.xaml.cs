using Microsoft.Extensions.Logging;
using Resources.Data;
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

            var logger = new LoggerFactory().CreateLogger<DatabaseService>();
            var dbService = new DatabaseService(logger);
            try
            {     
                // Initialize the database asynchronously
                Task.Run(async () => await dbService.InitializeAsync()).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {


                Debug.WriteLine("Exception initializing Database service", ex);
            }


            MainPage = new AppShell();
        }
    }
}
