using bookShoop.Data;
using BookShopping.Dto;

namespace BookShopping.Infrustructure.Abstruct
{
    public interface IBookRepository
    {
        Task AddBook(Book book);
        Task DeleteBook(Book book);
        Task<Book?> GetBookById(int id);
        Task<IEnumerable<Book>> GetBooks();
        Task UpdateBook(Book book);
        Task<IReadOnlyList<TopSellingBookModel>> GetTopSellingBooks(DateTime? startDate, DateTime? endDate, int topN);
    }
}
