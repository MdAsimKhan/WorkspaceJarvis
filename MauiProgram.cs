using MudBlazor.Services;
using WorkspaceJarvis.Data;
using WorkspaceJarvis.Services;

namespace WorkspaceJarvis
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<WorkspaceService>();
            builder.Services.AddMudServices();
            builder.Services.AddDbContext<AppDbContext>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            return builder.Build();
        }
    }
}
