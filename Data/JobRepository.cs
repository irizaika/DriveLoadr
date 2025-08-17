using DriveLoadr.Models;
using SQLite;

namespace DriveLoadr.Data
{
    public class JobRepository
    {
        private readonly SQLiteAsyncConnection _database;
        public JobRepository(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public Task<List<Job>> GetJobsInRangeAsync(DateTime start, DateTime end) =>
        _database.Table<Job>().Where(j => j.Date >= start && j.Date <= end).ToListAsync();
        public Task<int> SaveJobAsync(Job job) =>
            job.Id != 0 ? _database.UpdateAsync(job) : _database.InsertAsync(job);
        public Task<int> DeleteJobAsync(Job job) =>
            _database.DeleteAsync(job);
        public Task<List<Job>> GetDayJobs(DateTime day) =>
      _database.Table<Job>().Where(j => j.Date == day).ToListAsync();
    }
}
