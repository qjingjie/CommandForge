using CommandForge.Models;
using CommandForge.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CommandForge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructor
        public App()
        {
            IsQuit = false;

            // Allow UpdateSourceTrigger = PropertyChanged to input decimal values
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
        }
        #endregion

        #region Properties
        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public static bool IsQuit
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prepare and load necessary startup items.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServiceProvider = CreateServiceProvider();

            SetupApplication();

            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            Current.MainWindow = mainWindow;
            Current.MainWindow.Show();
        }

        /// <summary>
        /// Creates a service provider used to store registered services for dependency injection.
        /// </summary>
        /// <returns></returns>
        private IServiceProvider CreateServiceProvider()
        #endregion
        {
            IServiceCollection services = new ServiceCollection();

            #region Register Models
            services.AddSingleton<ConfigManager>();
            services.AddSingleton<ZmqCommunications>();
            #endregion

            #region Register ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ZmqCommunicationsViewModel>();
            #endregion

            #region Register Views
            services.AddSingleton<MainWindow>();
            #endregion

            #region Register Utilities
            #endregion

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Initialise application configuration, folders, loggers, and exception handling.
        /// </summary>
        private void SetupApplication()
        {
            // Create default folder in AppData to store application logs and configuration files
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CommandForge");
            string exceptionLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CommandForge", "Logs", "ErrorLog_.txt");
            string defaultLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CommandForge", "Logs", "CommandForge_.txt");


            // Load application configuration
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            ConfigManager configuration = ServiceProvider.GetRequiredService<ConfigManager>();

            bool configError = false;
            string configErrorMessage = string.Empty;

            try
            {
                configuration.LoadConfig("Config", Environment.CurrentDirectory);
            }
            catch (Exception e)
            {
                configError = true;
                configErrorMessage = e.Message;
            }

            // Setup serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(exceptionLogPath,
                              rollingInterval: RollingInterval.Hour,
                              restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal,
                              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{Newline}{Exception}",
                              retainedFileCountLimit: null)
                .WriteTo.File(defaultLogPath,
                              rollingInterval: RollingInterval.Hour,
                              restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{Newline}{Exception}",
                              retainedFileCountLimit: null)
                .CreateLogger();

            if (configError)
            {
                MessageBox.Show("The configuration file is invalid, the application will now exit.\n Please refer to the error logs for more details.",
                                "Invalid Configuration File",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Log.Fatal(configErrorMessage);
            }

            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.Fatal((Exception)e.ExceptionObject, "AppDomain.Current.UnhandledException" + "\n");
            DispatcherUnhandledException += (s, e) => Log.Fatal(e.Exception, "Application.Current.DispatcherUnhandledException" + "\n");
            TaskScheduler.UnobservedTaskException += (s, e) => Log.Fatal(e.Exception, "TaskScheduler.UnobservedTaskException" + "\n");
        }
    }
}