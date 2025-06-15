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
using System.Text.RegularExpressions;
using System.Windows;

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
            InitializeDecorationAssets();
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
            services.AddSingleton<ImageTransferManager>();

            services.AddSingleton<AssetManager>();
            services.AddSingleton<DecorationManager>();
            #endregion

            #region Core.Services
            services.AddSingleton<OpenCvAcquisitionService>();
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

        private static void InitializeDecorationAssets()
        {
            var assetManager = Services.GetRequiredService<AssetManager>();
            var decorationManager = Services.GetRequiredService<DecorationManager>();

            string decorationsRoot = "Assets/Decorations";
            var directoryPathes = Directory.GetDirectories(decorationsRoot);

            HashSet<FacePartType> firstPartChecker = [];
            foreach (var directoryPath in directoryPathes)
            {
                var imagePathes = Directory.GetFiles(directoryPath, "*.png");
                foreach (var imagePath in imagePathes)
                {
                    var fileName = Path.GetFileNameWithoutExtension(imagePath);
                    var assetId = fileName;

                    var match = Regex.Match(fileName, @"^(?<part>\w+)_(?<color>\w+)_(?<name>.+)$");
                    if (!match.Success)
                        continue;

                    if (!Enum.TryParse(match.Groups["part"].Value, ignoreCase: true, out FacePartType partType))
                        continue;
                    if (!Enum.TryParse(match.Groups["color"].Value, ignoreCase: true, out DecorationColor color))
                        continue;

                    assetManager.RegisterImage(assetId, imagePath);

                    string decoName = match.Groups["name"].Value;
                    string decoFullName = $"{decoName} ({color})";
                    string absoluteFilePath = Path.GetFullPath(imagePath);

                    if (partType is FacePartType.Eyes)
                        decorationManager.Put(assetId, decoFullName, absoluteFilePath, partType, color, scaleX: 1.8d, scaleY: 4d, firstPartChecker.Add(partType));
                    else
                        decorationManager.Put(assetId, decoFullName, absoluteFilePath, partType, color, scaleX: 1.6d, scaleY: 1.6d, firstPartChecker.Add(partType));
                }
            }
        }


        private static void InitializeEssentialComponents()
        {
            var imageProcessingManager = Services.GetRequiredService<ImageProcessingManager>();
            imageProcessingManager.Initialize();
            imageProcessingManager.ChangeSelectedCamera(0);
        }
        #endregion
    }
}
