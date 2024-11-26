using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Mapper.Services.LoggerService;
using Mapper.ViewModels;
using Mapper.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;

namespace Mapper
{
    public partial class App : Application
    {
        public IHost? GlobalHost { get; set; }

        private static readonly IHostBuilder _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {

                //config.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!)
                //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Register services and view models
                services.AddScoped<Views.MainWindow>();
                services.AddScoped<Views.MainView>();
                services.AddScoped<ViewModels.MainViewModel>();

                // Register DB context
                string? conString = context.Configuration.GetConnectionString("Default");

            });

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Build the Host and start the application
            GlobalHost = _host.Build();

            // Remove Avalonia's default data validation to avoid conflicts
            BindingPlugins.DataValidators.RemoveAt(0);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Use DI to resolve MainWindow and set its DataContext
                desktop.MainWindow = GlobalHost.Services.GetRequiredService<MainWindow>();
                desktop.MainWindow.DataContext = GlobalHost.Services.GetRequiredService<MainViewModel>();

                // Handle application exit
                desktop.Exit += (sender, args) =>
                {
                    GlobalHost?.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                    GlobalHost?.Dispose();
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                
                // Use DI for single view apps too
                singleViewPlatform.MainView = new MainView()
                {
                    DataContext = GlobalHost.Services.GetRequiredService<MainViewModel>()
                    
                };
                
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}

