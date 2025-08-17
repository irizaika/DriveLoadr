using DriveLoadr.Models;
using SQLite;

namespace DriveLoadr.Data
{
    public class VanRepository
    {
        private readonly SQLiteAsyncConnection _database;
        public VanRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public Task<List<Van>> GetVanListAsync() => _database.Table<Van>().ToListAsync();
        public Task<Van> GetVanfByIdAsync(int id) => _database.Table<Van>().Where(s => s.Id == id).FirstOrDefaultAsync();
        public Task<int> SaveVanAsync(Van van) => van.Id != 0 ? _database.UpdateAsync(van) : _database.InsertAsync(van);
        public Task<int> DeleteVanAsync(Van van) => _database.DeleteAsync(van);
    }
}
