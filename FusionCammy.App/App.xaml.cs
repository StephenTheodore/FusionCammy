using FusionCammy.App.ViewModels;
using FusionCammy.App.Views;
using FusionCammy.Core.Managers;
using FusionCammy.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FusionCammy.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();

            Services = BuildServiceProvider(serviceCollection);
            InitializeEssentialComponents();

            base.OnStartup(e);
        }

        private static ServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            #region Views & ViewModels
            services.AddSingleton<MainView>();

            services.AddTransient<CamView>();
            services.AddTransient<CamViewModel>();

            services.AddTransient<FunctionView>();
            services.AddTransient<FunctionViewModel>();
            #endregion

            #region Core.Managers
            services.AddSingleton<AssetManager>();
            services.AddSingleton<CameraManager>();
            services.AddSingleton<DecorationManager>();
            #endregion

            #region Core.Services
            services.AddSingleton<AcquisitionService>();
            services.AddSingleton<DecorationService>();
            #endregion

            return services.BuildServiceProvider();
        }

        private void InitializeEssentialComponents()
        {
            var assetManager = Services.GetRequiredService<AssetManager>();
            assetManager.Initialize();

            var cameraManager = Services.GetRequiredService<CameraManager>();
            cameraManager.Initialize();

            var decorationManager = Services.GetRequiredService<DecorationManager>();
            decorationManager.Initialize();
        }
    }
}
