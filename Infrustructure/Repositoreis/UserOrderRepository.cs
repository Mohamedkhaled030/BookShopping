using bookShoop.Application_Data;
using bookShoop.Data;
using BookShopping.Dto;
using BookShopping.Infrustructure.Abstruct;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Infrustructure.Repositoreis
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserOrderRepository(ApplicationDbContext db, IHttpContextAccessor httpContext, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpContext = httpContext;
            _userManager = userManager;
        }
        public async Task<Order?> GetOrderById(int id)
        {
            return await _db.Orders.FindAsync(id);
        }
        public string GetUserId()
        {
            var principal = _httpContext.HttpContext.User;
            if (principal == null)
                return "user Not Found";
            var userid = _userManager.GetUserId(principal);
            return userid;

        }
        public async Task<IEnumerable<Order>> GetAllUserOrders(bool getAll = false)
        {

            var order = _db.Orders
                           .Include(x => x.OrderStatus)
                           .Include(x => x.OrderDetail)
                           .ThenInclude(x => x.Book)
                           .ThenInclude(x => x.Genre).AsQueryable();

            if (!getAll)
            {
                var userid = GetUserId();
                if (string.IsNullOrEmpty(userid))
                    throw new Exception("User Not Loog-In");
                order = order.Where(x => x.UserId == userid);
                return await order.ToListAsync();
            }
            return await order.ToListAsync();

        }

        public async Task ChangeOrderStatus(UpdateOrderStatusDto data)
        {
            var order = await GetOrderById(data.OrderId);
            if (order == null)
                throw new Exception($"Order With ID : {data.OrderId} Dos Not Found");
            order.OrderStatusId = data.OrderId;
            await _db.SaveChangesAsync();
        }


        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await _db.orderStatuses.ToListAsync();
        }

        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await GetOrderById(orderId);
            if (order == null)
                throw new Exception($"Order With ID : {orderId} Dos Not Found");
            order.IsPaid = !order.IsPaid;
            await _db.SaveChangesAsync();
        }

    }
}
