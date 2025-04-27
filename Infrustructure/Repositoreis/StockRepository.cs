using bookShoop.Application_Data;
using bookShoop.Data;
using BookShopping.Dto;
using BookShopping.Infrustructure.Abstruct;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Infrustructure.Repositoreis
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _db;

        public StockRepository(ApplicationDbContext dbContext)
        {
            this._db = dbContext;
        }
        public async Task<Stock> GetStockById(int Bookid)
        {
            return await _db.Stocks.FirstOrDefaultAsync(x => x.BookId == Bookid);
        }
        public async Task MangeStock(StockDto stockDto)
        {
            var existStock = await GetStockById(stockDto.BookId);
            if (existStock == null)
            {
                var stocks = new Stock
                {
                    BookId = stockDto.BookId,
                    Quantity = stockDto.quantity,

                };
                _db.Stocks.Add(stocks);
                await _db.SaveChangesAsync();
            }
            else
            {
                existStock.Quantity = stockDto.quantity;
                await _db.SaveChangesAsync();
            }

        }
        public async Task<IEnumerable<StockDisplayDto>> GetStocks(string sTerm = "")
        {
            var Stocks = await (from book in _db.Books
                                join stock in _db.Stocks
                                on book.Id equals stock.BookId
                                into book_stock
                                from bookStock in book_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || book.BookName.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayDto
                                {
                                    BookId = book.Id,
                                    BookName = book.BookName,
                                    Quantity = bookStock == null ? 0 : bookStock.Quantity,
                                }
                       ).ToListAsync();
            return Stocks;
        }
    }
}
