using SQLite;

namespace DriveLoadr.Models
{
    public class Job
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int PartnerId { get; set; } = 0;
        public string CustomerName { get; set; } = ""; 
        public string Details { get; set; } = ""; 
        public string ContractorList { get; set; } = ""; // Could be JSON list of contractors ids or names
        public decimal Count { get; set; } = 1; // 1=full day, 0.5=half day etc.
        public decimal PayReceived { get; set; }
        public int JobTypeId { get; set; } = 0;
        public string VanList { get; set; } = "";

        // Optional helper properties for display
        [Ignore] public string LocationName { get; set; }
        [Ignore] public string ContractorsDisplay { get; set; }
        [Ignore] public string JobTypeName { get; set; }
        [Ignore] public string VanName { get; set; }
    }
}
