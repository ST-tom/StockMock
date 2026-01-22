namespace StockMock.Core.Configs
{
    public class DicItem : BaseAuditEntity
    {
        public long DicCategoryId { get; set; }

        public string CategoryName { get; set; }

        public long? ParentId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public string? Remark { get; set; }

        public string? ExtendInfo { get; set; }

        public string? CodePath { get; set; }

        public bool IsEnabled { get; set; } = true;

        public int Sort { get; set; }

        public DicItem? Parent { get; set; }

        public DicCategory DicCategory { get; set; }
    }
}
