using Agenda.Models;
using Agenda.Services;

namespace Agenda.Views
{
    public partial class EditarCitaPage : ContentPage
    {
        private readonly IDatabaseService _database;
        private readonly Citas _cita;

        public EditarCitaPage(IDatabaseService database, Citas cita)
        {
            InitializeComponent();
            _database = database;
            _cita = cita;
            BindingContext = _cita;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            await _database.UpdateCita(_cita);
            await Navigation.PopAsync();
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}