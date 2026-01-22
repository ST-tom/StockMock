namespace StockMock.Core.Configs
{
    public class DicCategory : BaseAuditEntity
    {
        public string Category { get; set; }

        public string DisplayName { get; set; }

        public bool IsEnabled { get; set; }

        public string Remark { get; set; }

        public int Sort { get; set; }

        public ICollection<DicItem> DicItems { get; set; } = [];
    }
}
