using Camera.MAUI;
using CommunityToolkit.Maui.Core;
using MauiScanner.Login;
using Microsoft.Extensions.Logging;

namespace MauiScanner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .ConfigureFonts( fonts =>
                {
                    fonts.AddFont( "OpenSans-Regular.ttf", "OpenSansRegular" );
                    fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
                } );
            //builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddTransient<LoginClass>();
            builder.Services.AddTransient<LoginPage>();

            builder.UseMauiApp<App>().UseMauiCommunityToolkitCore();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
