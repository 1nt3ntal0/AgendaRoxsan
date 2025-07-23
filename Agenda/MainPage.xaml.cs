using Agenda.Services;
using Agenda.Views;

namespace Agenda
{
    public partial class MainPage : ContentPage
    {
        private readonly IDatabaseService _database;

        public MainPage(IDatabaseService database)
        {
            InitializeComponent();
            _database = database;
        }

        private async void OpenShedulePage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgendaPage(_database));
        }

        private async void OpenWarehousePage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InventarioPage(_database));
        }
    }
}