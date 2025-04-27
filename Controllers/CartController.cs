
using bookShoop.Data;
using BookShopping.Dto;
using BookShopping.Infrustructure.Abstruct;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace bookShoop.Controllers
{

    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepo = cartRepository;
        }

        public async Task<IActionResult> AddItem(int bookId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(bookId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var cartCount = await _cartRepo.RemoveItem(bookId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            if (cart.ToString() == "Invalid")
                return LocalRedirect("/Identity/Account/Login");

            TempData["CartItem"] = JsonConvert.SerializeObject(cart, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });

            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto CheckDto)
        {
            if (!ModelState.IsValid)
                return View(CheckDto);
            var isCheakedOut = await _cartRepo.DoCheckout(CheckDto);
            if (!isCheakedOut)
                return RedirectToAction(nameof(OrderFailur));


            return RedirectToAction(nameof(OrderSuccess));

        }
        public IActionResult OrderSuccess()
        {
            return View();
        }
        public IActionResult OrderFailur()
        {
            return View();
        }
        public IActionResult Pay()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Cancel",
            };


            //var result = JsonConvert.DeserializeObject<IEnumerable<CartDetail>>((string)TempData["CartItem"]);
            var json = (string)TempData["CartItem"];
            var shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(json);

            if (shoppingCart?.CartDetails != null)
            {
                var result = shoppingCart.CartDetails; // استخراج قائمة CartDetails
                foreach (var model in result)
                {
                    var line = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "Usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = model.Book.BookName,
                                Description = model.Book.AuthorName,
                            },
                            UnitAmountDecimal = (decimal)model.UnitPrice,
                        },
                        Quantity = model.Quantity,
                    };
                    options.LineItems.Add(line);
                }
            }
            else
            {
                var result = new List<CartDetail>(); // إذا لم تكن هناك بيانات، تعيين قائمة فارغة
            }
            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);

        }
    }
}



