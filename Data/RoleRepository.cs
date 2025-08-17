using DriveLoadr.Models;
using SQLite;

namespace DriveLoadr.Data
{
    public class RoleRepository
    {
        private readonly SQLiteAsyncConnection _database;
        public RoleRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public Task<List<Role>> GetRolesAsync() => _database.Table<Role>().ToListAsync();


    }
}
