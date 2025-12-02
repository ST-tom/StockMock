namespace StockMock.Domain.Entities
{
    public class BaseAuditEntity : BaseEntity
    {
        public int? Creator { get; set; }

        public string? CreatorName { get; set; }

        public DateTime CreateTime { get; set; }

        public int? Updator { get; set; }

        public string? UpdatorName { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
