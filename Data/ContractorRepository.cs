using DriveLoadr.Models;
using SQLite;

namespace DriveLoadr.Data
{
    public class ContractorRepository
    {
        private readonly SQLiteAsyncConnection _database;
        public ContractorRepository(SQLiteAsyncConnection database) 
        {
            _database = database;
        }

        // Sample CRUD for Contractors
        public Task<List<Contractor>> GetContractorsAsync() => _database.Table<Contractor>().ToListAsync();
        public Task<Contractor> GetContractorByIdAsync(int id) => _database.Table<Contractor>().Where(s => s.Id == id).FirstOrDefaultAsync();
        public Task<int> SaveContractorAsync(Contractor contractor) => contractor.Id != 0 ? _database.UpdateAsync(contractor) : _database.InsertAsync(contractor);
        public Task<int> DeleteContractorAsync(Contractor contractor) => _database.DeleteAsync(contractor);

    }
}
