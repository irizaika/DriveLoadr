using SQLite;

namespace DriveLoadr.Models
{
    public class Role
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string RoleName { get; set; } = "";
    }
}
