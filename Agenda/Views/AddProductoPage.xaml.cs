using Agenda.Models;
using Agenda.Services;
using System.Windows.Input;

namespace Agenda.Views
{
    public partial class AddProductoPage : ContentPage
    {
        private readonly IDatabaseService _database;
        private readonly Producto _producto;

        public AddProductoPage(IDatabaseService database) : this(database, null) { }

        public AddProductoPage(IDatabaseService database, Producto producto)
        {
            InitializeComponent();
            _database = database;
            _producto = producto ?? new Producto();

            // Si es nuevo, dejar los valores como vacío
            if (producto == null)
            {
                CostoString = "";
                CantidadString = "";
                TipoSeleccionado = TipoProducto.Uso;
            }
            else
            {
                CostoString = _producto.Costo.ToString();
                CantidadString = _producto.Cantidad.ToString();
                TipoSeleccionado = _producto.Tipo;
            }

            GuardarCommand = new Command(async () => await GuardarProducto());
            BindingContext = this;
        }

        public ICommand GuardarCommand { get; }

        // Propiedades para los Entry
        public string Nombre
        {
            get => _producto.Nombre;
            set => _producto.Nombre = value;
        }

        public string Marca
        {
            get => _producto.Marca;
            set => _producto.Marca = value;
        }

        public string CostoString { get; set; }
        public string CantidadString { get; set; }

        public TipoProducto TipoSeleccionado { get; set; }
        public IEnumerable<TipoProducto> TiposProducto => Enum.GetValues(typeof(TipoProducto)).Cast<TipoProducto>();

        private async Task GuardarProducto()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Nombre))
                {
                    await DisplayAlert("Error", "El nombre es requerido", "OK");
                    return;
                }

                if (!decimal.TryParse(CostoString, out decimal costo))
                {
                    await DisplayAlert("Error", "Costo inválido", "OK");
                    return;
                }

                if (!int.TryParse(CantidadString, out int cantidad))
                {
                    await DisplayAlert("Error", "Cantidad inválida", "OK");
                    return;
                }

                _producto.Costo = costo;
                _producto.Cantidad = cantidad;
                _producto.Tipo = TipoSeleccionado;

                if (_producto.Id == 0)
                    await _database.AddProducto(_producto);
                else
                    await _database.UpdateProducto(_producto);

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
            }
        }
    }
}
