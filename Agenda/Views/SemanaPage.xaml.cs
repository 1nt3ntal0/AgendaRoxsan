using Agenda.Models;
using Agenda.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Agenda.Views
{
    public partial class SemanaPage : ContentPage
    {
        private readonly IDatabaseService _database;
        private readonly ObservableCollection<ResumenServicio> _resumenServicios;

        public class ResumenServicio
        {
            public string Servicio { get; set; }
            public int Cantidad { get; set; }
        }

        public SemanaPage(IDatabaseService database)
        {
            InitializeComponent();
            _database = database;
            _resumenServicios = new ObservableCollection<ResumenServicio>();
            ResumenCollection.ItemsSource = _resumenServicios;

            this.Appearing += async (s, e) => await CargarResumenSemanal();
        }

        private async Task CargarResumenSemanal()
        {
            try
            {
                IsBusy = true;
                await _database.InitializeDatabase();
                var todasCitas = await _database.GetCitas();

                // Obtener el primer y último día de la semana actual (lunes a sábado)
                var hoy = DateTime.Today;
                var lunes = hoy.AddDays(-(int)hoy.DayOfWeek + (int)DayOfWeek.Monday);
                var sabado = lunes.AddDays(5);

                // Filtrar citas de esta semana (lunes a sábado)
                var citasSemana = todasCitas
                    .Where(c => c.Dia >= lunes && c.Dia <= sabado && c.Completada)
                    .ToList();

                // Agrupar por servicio y contar
                var resumen = citasSemana
                    .GroupBy(c => c.Servicio)
                    .Select(g => new ResumenServicio
                    {
                        Servicio = g.Key,
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(r => r.Cantidad)
                    .ThenBy(r => r.Servicio)
                    .ToList();

                _resumenServicios.Clear();
                foreach (var item in resumen)
                {
                    _resumenServicios.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cargando resumen semanal: {ex}");
                await DisplayAlert("Error", "No se pudo cargar el resumen semanal", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnHoyClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgendaPage(_database));
        }

        private async void OnAgendarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgendarPage(_database));
        }

        private void OnSemanaClicked(object sender, EventArgs e)
        {
            // Ya estamos en SemanaPage
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}