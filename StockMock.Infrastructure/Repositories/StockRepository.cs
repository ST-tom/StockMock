using StockMock.Infrastructure.Database;

namespace StockMock.Infrastructure.Repositories
{
    public class StockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
