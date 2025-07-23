using Agenda.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Agenda
{
    public partial class App : Application
    {
        public App(IDatabaseService databaseService)
        {
            InitializeComponent();

            // Inicialización asíncrona de la base de datos
            Task.Run(async () => {
                try
                {
                    await databaseService.InitializeDatabase();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inicializando BD: {ex}");
                }
            }).Wait();

            // Registrar rutas
            Routing.RegisterRoute(nameof(Views.AgendaPage), typeof(Views.AgendaPage));
            Routing.RegisterRoute(nameof(Views.AgendarPage), typeof(Views.AgendarPage));
            Routing.RegisterRoute(nameof(Views.SemanaPage), typeof(Views.SemanaPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

            MainPage = new NavigationPage(new MainPage(databaseService));
        }
    }
}