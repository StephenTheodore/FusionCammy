using FusionCammy.App.Managers;
using FusionCammy.App.ViewModels;
using FusionCammy.App.Views;
using FusionCammy.Core.Configurations;
using FusionCammy.Core.Managers;
using FusionCammy.Core.Models;
using FusionCammy.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace FusionCammy.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Field
        private const string configPath = "appsettings.json";

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };
        #endregion

        #region Property
        public static IServiceProvider Services { get; private set; } = null!;
        #endregion

        #region Method
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
            services.AddTransient<CamView>();
            services.AddTransient<CamViewModel>();

            services.AddTransient<FunctionView>();
            services.AddTransient<FunctionViewModel>();
            #endregion

            #region Core.Managers
            services.AddSingleton<ImageProcessingManager>();

            services.AddSingleton<AssetManager>();
            services.AddSingleton<DecorationManager>();
            #endregion

            #region Core.Services
            services.AddSingleton<AcquisitionService>();
            services.AddSingleton<DecorationService>();
            services.AddSingleton<FacialAnalysisService>();
            #endregion

            #region Configurations
            VerifyConfigurationFile();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();

            services.Configure<MlConfiguration>(configuration.GetSection(nameof(MlConfiguration)));
            #endregion

            return services.BuildServiceProvider();
        }

        private static void VerifyConfigurationFile()
        {

            if (!File.Exists(configPath))
            {
                var newConfig = new
                {
                    MlConfiguration = new
                    {
                        FacialDetectionModelPath = "Assets/MlModels/shape_predictor_68_face_landmarks.dat"
                    }
                };

                var json = JsonSerializer.Serialize(newConfig, jsonOptions);
                File.WriteAllText(configPath, json);
            }
        }

        private static void InitializeEssentialComponents()
        {
            var assetManager = Services.GetRequiredService<AssetManager>();
            // TODO : 전체 애셋 로딩, 분류 자동화
            assetManager.RegisterImage($"{Colors.Red}_{FacePartType.Nose}_Ball", @"Assets\Decorations\Nose\Ball_Red.png");

            var decorationManager = Services.GetRequiredService<DecorationManager>();
            decorationManager.Put($"{Colors.Red}_{FacePartType.Nose}_Ball", FacePartType.Nose, 1.5d, true);

            var imageProcessingManager = Services.GetRequiredService<ImageProcessingManager>();
            imageProcessingManager.Initialize();
        }
        #endregion
    }
}
