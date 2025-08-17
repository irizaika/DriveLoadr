using DriveLoadr.Models;
using SQLite;

namespace DriveLoadr.Data
{
    public class PartnerRepository
    {
        private readonly SQLiteAsyncConnection _database;
        public PartnerRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public Task<List<Partner>> GetPartnersListAsync() => _database.Table<Partner>().ToListAsync();
        public Task<Partner> GetPartnerByIdAsync(int id) => _database.Table<Partner>().Where(s => s.Id == id).FirstOrDefaultAsync();
        public Task<int> SavePartnerAsync(Partner p) => p.Id != 0 ? _database.UpdateAsync(p) : _database.InsertAsync(p);
        public Task<int> DeletePartnerAsync(Partner p) => _database.DeleteAsync(p);
    }
}
