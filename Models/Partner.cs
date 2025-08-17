using SQLite;

namespace DriveLoadr.Models
{
    public class Partner
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string CompanyName { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone1 { get; set; } = "";
        public string Phone2 { get; set; } = "";
        public string Email { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
