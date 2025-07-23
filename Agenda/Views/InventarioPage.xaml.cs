using Agenda.Models;
using Agenda.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Agenda.Views
{
    public partial class InventarioPage : ContentPage
    {
        private readonly IDatabaseService _database;
        private readonly ObservableCollection<Producto> _productos;
        private bool _mostrarPrecios = true;

        public InventarioPage(IDatabaseService database)
        {
            InitializeComponent();
            _database = database;
            _productos = new ObservableCollection<Producto>();
            ProductosCollection.ItemsSource = _productos;

            // Inicializar comandos
            CargarProductosCommand = new Command(async () => await CargarProductos());
            TogglePreciosCommand = new Command(TogglePrecios);
            AgregarProductoCommand = new Command(async () => await AgregarProducto());
            EditarProductoCommand = new Command<Producto>(async (p) => await EditarProducto(p));
            EliminarProductoCommand = new Command<Producto>(async (p) => await EliminarProducto(p));

            this.Appearing += async (s, e) => await CargarProductos();
        }

        public ICommand CargarProductosCommand { get; }
        public ICommand TogglePreciosCommand { get; }
        public ICommand AgregarProductoCommand { get; }
        public ICommand EditarProductoCommand { get; }
        public ICommand EliminarProductoCommand { get; }

        public bool MostrarPrecios
        {
            get => _mostrarPrecios;
            set
            {
                if (_mostrarPrecios != value)
                {
                    _mostrarPrecios = value;
                    OnPropertyChanged(nameof(MostrarPrecios));
                    OnPropertyChanged(nameof(MostrarPreciosText));

                    foreach (var producto in _productos)
                    {
                        producto.MostrarPrecios = value;
                    }
                }
            }
        }

        public string MostrarPreciosText => MostrarPrecios ? "Ocultar Precios" : "Mostrar Precios";

        private async Task CargarProductos()
        {
            try
            {
                IsBusy = true;
                await _database.InitializeDatabase();
                var productos = await _database.GetProductos();

                _productos.Clear();
                foreach (var producto in productos.Where(p => p.Tipo == TipoProducto.Uso))
                {
                    producto.MostrarPrecios = MostrarPrecios;
                    _productos.Add(producto);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar productos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void TogglePrecios()
        {
            MostrarPrecios = !MostrarPrecios;
        }

        private async Task AgregarProducto()
        {
            await Navigation.PushAsync(new AddProductoPage(_database));
        }

        private async Task EditarProducto(Producto producto)
        {
            if (producto == null) return;
            await Navigation.PushAsync(new AddProductoPage(_database, producto));
        }

        private async Task EliminarProducto(Producto producto)
        {
            if (producto == null) return;

            bool confirm = await DisplayAlert("Confirmar",
                $"¿Eliminar el producto {producto.Nombre}?", "Sí", "Cancelar");

            if (confirm)
            {
                try
                {
                    await _database.DeleteProducto(producto);
                    _productos.Remove(producto);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
                }
            }
        }
        private async void OnCargarClicked(object sender, EventArgs e)
        {
            await CargarProductos();
        }


        private async void OnVentaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InventarioVentaPage(_database));
        }

        private async void OnAgregarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddProductoPage(_database));
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}