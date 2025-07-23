using Agenda.Services;
using Microsoft.Extensions.Logging;

namespace Agenda
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Registrar servicios
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<Views.AgendaPage>();
            builder.Services.AddTransient<Views.AgendarPage>();
            builder.Services.AddTransient<Views.SemanaPage>();
            builder.Services.AddTransient<Views.InventarioPage>();
            builder.Services.AddTransient<Views.InventarioPage>();
            builder.Services.AddTransient<Views.InventarioVentaPage>();
            builder.Services.AddTransient<Views.AddProductoPage>();

            return builder.Build();
        }
    }
}