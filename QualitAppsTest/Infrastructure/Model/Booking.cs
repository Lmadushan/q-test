namespace QualitAppsTest.Infrastructure.Models
{
    public class Booking
    {
        public long? Id { get; set; }
        public string? FromAddress { get; set; }
        public string? Status { get; set; }
        public string? ToAddress { get; set; }
        public string? Type { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Price { get; set; }
        public string? AssigendDriver { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
