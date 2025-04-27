using bookShoop.Data;
using BookShopping.Dto;

namespace BookShopping.Infrustructure.Abstruct
{
    public interface IStockRepository
    {
        Task<Stock> GetStockById(int Bookid);
        Task<IEnumerable<StockDisplayDto>> GetStocks(string sTerm = "");
        Task MangeStock(StockDto stockDto);
    }
}
