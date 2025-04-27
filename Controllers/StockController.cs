using bookShoop.Constant;
using bookShoop.Data;
using BookShopping.Dto;
using BookShopping.Infrustructure.Abstruct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShopping.Controllers
{
    [Authorize(Roles = (nameof(Roles.Admin)))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepository;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }
        public async Task<IActionResult> Index(string sTrem = "")
        {
            var stocks = await _stockRepository.GetStocks(sTrem);
            return View(stocks);
        }
        [HttpGet]
        public async Task<IActionResult> MangeStock(int bookid)
        {
            var existstock = await _stockRepository.GetStockById(bookid);
            var stock = new Stock
            {
                BookId = bookid,
                Quantity = existstock != null ? existstock.Quantity : 0
            };
            return View(stock);
        }
        [HttpPost]
        public async Task<IActionResult> MangeStock(StockDto stock)
        {
            if (!ModelState.IsValid)
                return View(stock);
            try
            {
                await _stockRepository.MangeStock(stock);
                TempData["successMessage"] = "Stock is updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Something went wrong!!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
