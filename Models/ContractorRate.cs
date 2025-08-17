using SQLite;

namespace DriveLoadr.Models
{
    public class ContractorRate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ContractorId { get; set; }
        public int JobTypeId { get; set; }
        decimal Pay { get; set; }
    }
}
