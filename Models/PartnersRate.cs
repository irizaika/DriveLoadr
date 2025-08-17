using SQLite;

namespace DriveLoadr.Models
{
    public class PartnersRate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public int JobTypeId { get; set; }
        decimal Pay {  get; set; }
    }
}
