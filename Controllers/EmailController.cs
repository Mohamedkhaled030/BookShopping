using BookShopping.Infrustructure.Abstruct;
using Microsoft.AspNetCore.Mvc;

namespace BookShopping.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailSender _emailSender;
        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
