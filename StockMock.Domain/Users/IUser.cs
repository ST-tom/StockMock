namespace StockMock.Domain.Users
{
    public interface IUser
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
