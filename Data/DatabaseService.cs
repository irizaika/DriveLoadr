using SQLite;
using DriveLoadr.Models;

namespace DriveLoadr.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
        }

        public async Task Init()
        {
            try
            {
                await _database.CreateTableAsync<Contractor>();
                await _database.CreateTableAsync<Role>();
                await _database.CreateTableAsync<JobType>();
                await _database.CreateTableAsync<Partner>();
                await _database.CreateTableAsync<Van>();
                await _database.CreateTableAsync<Job>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
            }

            // Seed roles and job types if empty
            if ((await _database.Table<Role>().CountAsync()) == 0)
            {
                await _database.InsertAsync(new Role { RoleName = "Driver" });
                await _database.InsertAsync(new Role { RoleName = "Porter" });
            }
            if ((await _database.Table<JobType>().CountAsync()) == 0)
            {
                await _database.InsertAsync(new JobType
                {
                    Name = "2 man+van",
                    PartnerID = 0,
                    Description = "General 2 man crew with van",
                    NumberOfPeople = 2,
                    NumberOfVans = 1,
                    PayRate = 500
                });
                await _database.InsertAsync(new JobType { Name = "1 man+van",
                    PartnerID = 0,
                    Description = "General 1 man job with  van",
                    NumberOfPeople = 1,
                    NumberOfVans = 1,
                    PayRate = 250
                });
                await _database.InsertAsync(new JobType { Name = "1 man",
                    PartnerID = 0,
                    Description = "General 1 man job",
                    NumberOfPeople = 1,
                    NumberOfVans = 0,
                    PayRate = 150
                });
            }
        }
    }
}
