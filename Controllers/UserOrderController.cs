
using BookShopping.Infrustructure.Abstruct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bookShoop.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrder;

        public UserOrderController(IUserOrderRepository userOrder)
        {
            _userOrder = userOrder;
        }
        public async Task<IActionResult> UserOrder()
        {
            var Order = await _userOrder.GetAllUserOrders();
            return View(Order);
        }
    }
}
