using bookShoop.Data;
using bookShoop.Dto;
using BookShopping.Infrustructure.Abstruct;
using BookShopping.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace bookShoop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHomeRepository homeRepository, ILogger<HomeController> logger)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0)
        {
            IEnumerable<Book> books = await _homeRepository.GetBook(sterm, genreId);
            IEnumerable<Genre> genres = await _homeRepository.GetGenre();
            var BookDto = new BooKDisplayDto
            {
                Books = books,
                Genres = genres,
                GenreId = genreId,
                STerm = sterm

            };
            return View(BookDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
