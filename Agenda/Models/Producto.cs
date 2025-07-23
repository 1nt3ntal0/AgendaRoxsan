using SQLite;
using System.ComponentModel;

namespace Agenda.Models
{
    public enum TipoProducto
    {
        Uso,
        Venta
    }

    public class Producto : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public decimal Costo { get; set; }
        public int Cantidad { get; set; }
        public string Foto { get; set; }
        public TipoProducto Tipo { get; set; }

        private bool _mostrarPrecios = true;
        public bool MostrarPrecios
        {
            get => _mostrarPrecios;
            set
            {
                if (_mostrarPrecios != value)
                {
                    _mostrarPrecios = value;
                    OnPropertyChanged(nameof(MostrarPrecios));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}