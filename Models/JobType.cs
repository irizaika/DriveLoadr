using SQLite;

namespace DriveLoadr.Models
{
    public class JobType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int NumberOfPeople { get; set; }
        public int NumberOfVans { get; set; }
        public int PartnerID { get; set; }
        public decimal PayRate { get; set; }

    }
}
