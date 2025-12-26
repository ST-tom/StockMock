namespace StockMock.Core.Configs
{
    public class Day : BaseEntity
    {
        public DateOnly Date { get; set; }

        public bool IsWorkDay { get; set; }
    }
}
