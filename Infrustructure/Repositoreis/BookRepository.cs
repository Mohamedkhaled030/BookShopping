using bookShoop.Application_Data;
using bookShoop.Data;
using BookShopping.Dto;
using BookShopping.Infrustructure.Abstruct;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StoredProcedureEFCore;

namespace BookShopping.Infrustructure.Repositoreis
{


    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TopSellingBookModel>> GetTopSellingBooks(DateTime? startDate, DateTime? endDate, int topN)

        {
            DateTime defaultStartDate = startDate ?? DateTime.UtcNow.AddDays(-30);
            DateTime defaultEndDate = endDate ?? DateTime.Now;


            var result = await _context.TopSellingBooks
                .FromSqlRaw("EXEC GetTopSellingBooks @StartDate, @EndDate, @TopN",
                    new SqlParameter("@StartDate", defaultStartDate),
                    new SqlParameter("@EndDate", defaultEndDate),
                    new SqlParameter("@TopN", topN))
                .ToListAsync();

            return result;
        }
        public async Task AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Book?> GetBookById(int id) => await _context.Books.FindAsync(id);

        public async Task<IEnumerable<Book>> GetBooks() => await _context.Books.Include(a => a.Genre).ToListAsync();
    }
}


