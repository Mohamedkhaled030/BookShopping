using bookShoop.Application_Data;
using bookShoop.Data;
using BookShopping.Infrustructure.Abstruct;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Infrustructure.Repositoreis
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Book>> GetBook(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();
            IQueryable<Book> Book = from book in _db.Books
                                    join genre in _db.Genres
                                    on book.GenreId equals genre.Id
                                    join stock in _db.Stocks
                                    on book.Id equals stock.BookId
                                    into book_stocks
                                    from bookWithStock in book_stocks.DefaultIfEmpty()
                                    where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.BookName.ToLower().StartsWith(sTerm))
                                    where string.IsNullOrWhiteSpace(sTerm) || book != null && sTerm.ToLower().StartsWith(sTerm)
                                    select new Book
                                    {
                                        Id = book.Id,
                                        Image = book.Image,
                                        AuthorName = book.AuthorName,
                                        BookName = book.BookName,
                                        GenreId = book.GenreId,
                                        Price = book.Price,
                                        GenreName = genre.GenreName,
                                        Quantity = bookWithStock != null ? bookWithStock.Quantity : 0,

                                    };
            if (genreId > 0)
            {
                Book = Book.Where(x => x.GenreId == genreId);
            }
            return await Book.ToListAsync();
        }
        public async Task<IEnumerable<Genre>> GetGenre()
        {
            return await _db.Genres.ToListAsync();
        }

    }
}
