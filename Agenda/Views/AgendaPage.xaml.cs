using Agenda.Models;
using Agenda.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Agenda.Views
{
    public partial class AgendaPage : ContentPage
    {
        private readonly IDatabaseService _database;
        private readonly ObservableCollection<Citas> _citasHoy;

        public AgendaPage(IDatabaseService database)
        {
            InitializeComponent();
            _database = database;
            _citasHoy = new ObservableCollection<Citas>();
            CitasCollection.ItemsSource = _citasHoy;

            this.Appearing += async (s, e) => await CargarCitas();
        }

        private async Task CargarCitas()
        {
            try
            {
                IsBusy = true;
                await _database.InitializeDatabase();
                var todasCitas = await _database.GetCitas();
                var citasHoy = todasCitas
                    .Where(c => c.Dia.Date == DateTime.Today)
                    .OrderBy(c => c.Hora)
                    .ToList();

                _citasHoy.Clear();
                foreach (var cita in citasHoy)
                {
                    _citasHoy.Add(cita);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cargando citas: {ex}");
                await DisplayAlert("Error", "No se pudieron cargar las citas", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Manejador para el cambio de estado "Completada"
        private async void OnCompletadaChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is Citas cita)
            {
                try
                {
                    cita.Completada = e.Value;
                    await _database.UpdateCita(cita);

                    // Opcional: Forzar actualización de la vista
                    var temp = _citasHoy.ToList();
                    _citasHoy.Clear();
                    foreach (var item in temp)
                    {
                        _citasHoy.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error actualizando cita: {ex}");
                    await DisplayAlert("Error", "No se pudo actualizar el estado", "OK");
                    // Revertir el cambio si falla
                    checkBox.IsChecked = !e.Value;
                }
            }
        }

        private void OnCitaSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Citas cita)
            {
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private async void OnHoyClicked(object sender, EventArgs e)
        {
            await CargarCitas();
        }

        private async void OnAgendarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgendarPage(_database));
        }

        private async void OnSemanaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SemanaPage(_database));
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnEditarClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Citas cita)
            {
                await Navigation.PushAsync(new EditarCitaPage(_database, cita));
            }
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Citas cita)
            {
                bool confirmar = await DisplayAlert("Confirmar",
                    $"¿Eliminar cita con {cita.Nombre}?", "Sí", "No");

                if (confirmar)
                {
                    await _database.DeleteCita(cita);
                    await CargarCitas();
                }
            }
        }
    }

    public class CompletadaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Colors.Gray : Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}