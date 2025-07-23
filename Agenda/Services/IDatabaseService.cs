using Agenda.Models;

namespace Agenda.Services
{
    public interface IDatabaseService
    {
        Task InitializeDatabase();
        Task<List<Citas>> GetCitas();
        Task<int> AddCita(Citas cita);
        Task<int> UpdateCita(Citas cita);
        Task<int> DeleteCita(Citas cita);

        Task<List<Producto>> GetProductos();
        Task<int> AddProducto(Producto producto);
        Task<int> UpdateProducto(Producto producto);
        Task<int> DeleteProducto(Producto producto);
    }
}