using Agenda.Models;
using Agenda.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Agenda.Views
{
    public partial class AgendarPage : ContentPage
    {
        private readonly IDatabaseService _database;
        private Citas _nuevaCita;

        public ObservableCollection<string> ServiciosDisponibles { get; } = new ObservableCollection<string>
        {
            "Corte de cabello",
            "Coloración",
            "Tratamiento capilar",
            "Manicure",
            "Pedicure",
            "Maquillaje"
        };

        public DateTime FechaMinima => DateTime.Today;
        public DateTime FechaMaxima => DateTime.Today.AddMonths(3);

        public ICommand GuardarCitaCommand { get; }

        public AgendarPage(IDatabaseService database)
        {
            InitializeComponent();
            _database = database;
            _nuevaCita = new Citas
            {
                Dia = DateTime.Today,
                Hora = new TimeSpan(10, 0, 0),
                Completada = false
            };

            GuardarCitaCommand = new Command(async () => await GuardarCita());

            BindingContext = this;
        }

        public string Nombre
        {
            get => _nuevaCita.Nombre;
            set => _nuevaCita.Nombre = value;
        }

        public string Servicio
        {
            get => _nuevaCita.Servicio;
            set => _nuevaCita.Servicio = value;
        }

        public DateTime Dia
        {
            get => _nuevaCita.Dia;
            set => _nuevaCita.Dia = value;
        }

        public TimeSpan Hora
        {
            get => _nuevaCita.Hora;
            set => _nuevaCita.Hora = value;
        }

        private async Task GuardarCita()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_nuevaCita.Nombre))
                {
                    await DisplayAlert("Error", "Debe ingresar un nombre", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_nuevaCita.Servicio))
                {
                    await DisplayAlert("Error", "Debe seleccionar un servicio", "OK");
                    return;
                }

                var fechaHoraCita = _nuevaCita.Dia.Date + _nuevaCita.Hora;

                if (fechaHoraCita < DateTime.Now.AddMinutes(30))
                {
                    await DisplayAlert("Error", "La cita debe ser al menos 30 minutos después del momento actual.", "OK");
                    return;
                }

                await _database.AddCita(_nuevaCita);
                await DisplayAlert("Éxito", "Cita agendada correctamente", "OK");
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar la cita: {ex.Message}", "OK");
            }
        }


        private void LimpiarFormulario()
        {
            _nuevaCita = new Citas
            {
                Dia = DateTime.Today,
                Hora = new TimeSpan(10, 0, 0),
                Completada = false
            };

            OnPropertyChanged(nameof(Nombre));
            OnPropertyChanged(nameof(Servicio));
            OnPropertyChanged(nameof(Dia));
            OnPropertyChanged(nameof(Hora));
        }

        private async void OnHoyClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgendaPage(_database));
        }

        private async void OnAgendarClicked(object sender, EventArgs e)
        {
            // Ya estamos en AgendarPage
        }

        private async void OnSemanaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SemanaPage(_database));
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}