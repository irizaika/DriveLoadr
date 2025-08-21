//using CloudKit;
using DriveLoadr.Services;

namespace DriveLoadr
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }
        public App()
        {
            InitializeComponent();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "logistics.db3");
            Database = new DatabaseService(dbPath);

            InitializeDatabaseAsync();
        }


        private async void InitializeDatabaseAsync()
        {
            try
            {
                await Database.Init();
                Console.WriteLine("Database initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing DB: {ex.Message}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}