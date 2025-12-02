using StockMock.Infrastructure.Database;

namespace StockMock.Infrastructure.Repositories
{
    public class MockRepository
    {
        private readonly ApplicationDbContext _context;

        public MockRepository(ApplicationDbContext context)
        {
            _context = context; 
        }


    }
}
