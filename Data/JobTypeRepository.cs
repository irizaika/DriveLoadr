using DriveLoadr.Models;
using SQLite;

namespace DriveLoadr.Data
{
    public class JobTypeRepository
    {
        private readonly SQLiteAsyncConnection _database;
        public JobTypeRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }
        public Task<List<JobType>> GetJobTypesAsync() => _database.Table<JobType>().ToListAsync();
        public Task<JobType> GetJobfByIdAsync(int id) => _database.Table<JobType>().Where(s => s.Id == id).FirstOrDefaultAsync();
        public Task<int> SaveJobTypeAsync(JobType van) => van.Id != 0 ? _database.UpdateAsync(van) : _database.InsertAsync(van);
        public Task<int> DeleteJobTypeAsync(JobType van) => _database.DeleteAsync(van);
        //public Task<JobType> GetJobfByIdAsync(int id) => _database.Table<JobType>().Where(s => s.Id == id).FirstOrDefaultAsync();

    }
}
