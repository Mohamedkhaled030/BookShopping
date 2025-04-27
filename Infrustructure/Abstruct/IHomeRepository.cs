using bookShoop.Data;

namespace BookShopping.Infrustructure.Abstruct
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBook(string sTierm = "", int genreId = 0);
        Task<IEnumerable<Genre>> GetGenre();
    }
}
