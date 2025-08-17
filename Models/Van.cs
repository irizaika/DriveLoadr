using SQLite;

namespace DriveLoadr.Models
{
    public class Van
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string VanName { get; set; } = "";
        public string Details { get; set; } = "";
        public string Plate { get; set; } = "";
    }
}
