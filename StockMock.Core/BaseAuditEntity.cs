namespace StockMock.Core
{
    public class BaseAuditEntity : BaseEntity
    {
        public long? Creator { get; set; }

        public string? CreatorName { get; set; }

        public DateTime CreateTime { get; set; }

        public long? Updator { get; set; }

        public string? UpdatorName { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
