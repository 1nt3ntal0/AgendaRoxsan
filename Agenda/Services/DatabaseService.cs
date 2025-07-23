using SQLite;
using Agenda.Models;
using System.Diagnostics;

namespace Agenda.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection _db;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public DatabaseService()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AgendaDB.db3");
            _db = new SQLiteAsyncConnection(databasePath);
        }

        public async Task InitializeDatabase()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_isInitialized)
                {
                    await _db.CreateTableAsync<Citas>();
                    await _db.CreateTableAsync<Producto>();
                    _isInitialized = true;
                    Debug.WriteLine("Base de datos inicializada correctamente");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al inicializar BD: {ex}");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<Citas>> GetCitas()
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.Table<Citas>().ToListAsync();
        }

        public async Task<int> AddCita(Citas cita)
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.InsertAsync(cita);
        }

        public async Task<int> UpdateCita(Citas cita)
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.UpdateAsync(cita);
        }

        public async Task<int> DeleteCita(Citas cita)
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.DeleteAsync(cita);
        }

        // Operaciones para Productos (mismo patrón)
        public async Task<List<Producto>> GetProductos()
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.Table<Producto>().ToListAsync();
        }

        public async Task<int> AddProducto(Producto producto)
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.InsertAsync(producto);
        }

        public async Task<int> UpdateProducto(Producto producto)
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.UpdateAsync(producto);
        }

        public async Task<int> DeleteProducto(Producto producto)
        {
            if (!_isInitialized)
                await InitializeDatabase();

            return await _db.DeleteAsync(producto);
        }
    }
}