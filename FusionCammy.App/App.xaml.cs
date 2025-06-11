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
        public static IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();

            AddServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();

            base.OnStartup(e);
        }

        private static void AddServices(IServiceCollection services)
        {
            #region ViewModels
            services.AddSingleton<MainView>();
            services.AddSingleton<MainViewModel>();
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
        }
    }
}
