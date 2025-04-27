using bookShoop.Application_Data;
using bookShoop.Data;
using BookShopping.Dto;
using BookShopping.Infrustructure.Abstruct;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShopping.Infrustructure.Repositoreis
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<object> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                return "InvalidUser";
            //throw new Exception("Invalid User");
            var shoppingCart = await _db.ShoppingCarts
                                        .Include(a => a.CartDetails)
                                        .ThenInclude(a => a.Book)
                                        .ThenInclude(a => a.Stock)
                                        .Include(a => a.CartDetails)
                                        .ThenInclude(a => a.Book)
                                        .ThenInclude(a => a.Genre)
                                        .Where(a => a.UserId == userId).FirstOrDefaultAsync();

            if (shoppingCart == null)
                return "CartEmpty";




            return shoppingCart;
        }
        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price  // it is a new line after update
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> GetCartItemCount(string userid = "")
        {
            if (string.IsNullOrEmpty(userid))
                userid = GetUserId();

            var CountItem = await (from cart in _db.ShoppingCarts
                                   join cartDetail in _db.CartDetails
                                   on cart.Id equals cartDetail.ShoppingCartId
                                   where cart.UserId == userid // updated line
                                   select new { cartDetail.Id }
                        ).ToListAsync();
            return CountItem.Count;

        }
        public async Task<int> RemoveItem(int bookId)
        {
            string userid = GetUserId();
            using var trancaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userid))
                    throw new Exception("user Is Not Logged-In");

                var cart = await GetCart(userid);
                if (cart == null)
                    throw new Exception("Invalid Empty");


                //cart detals siction

                var cartItem = _db.CartDetails.FirstOrDefault(x => x.ShoppingCartId == cart.Id && x.BookId == bookId);
                if (cartItem == null)
                    throw new Exception("Not Item in Cart");

                if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                {
                    cartItem.Quantity = cartItem.Quantity - 1;

                }
                _db.SaveChanges();
                trancaction.Commit();

            }
            catch (Exception ex)
            {
                trancaction.Rollback();

            }
            var totalItemCount = await GetCartItemCount(userid);
            return totalItemCount;
        }
        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }
        public async Task<bool> DoCheckout(CheckoutDto CheckDto)
        {
            var trancaction = _db.Database.BeginTransaction();
            try
            {
                //try to move data from cart detials to order detials
                var userid = GetUserId();
                if (userid == null)
                    throw new Exception("User Not Logg-in");
                var cart = await GetCart(userid);
                if (cart == null)
                    throw new Exception("Invalid cart");
                var cartdetails = _db.CartDetails.Where(x => x.ShoppingCartId == cart.Id).ToList();
                if (cartdetails == null)
                    throw new Exception("Cart Is Epmty");
                var pendingRecord = _db.orderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord is null)
                    throw new InvalidOperationException("Order status does not have Pending status");
                var order = new Order()
                {
                    UserId = userid,
                    CreateDate = DateTime.UtcNow,
                    Name = CheckDto.Name,
                    Email = CheckDto.Email,
                    MobileNumber = CheckDto.MobileNumber,
                    PaymentMethod = CheckDto.PaymentMethod,
                    Address = CheckDto.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id


                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var item in cartdetails)
                {
                    var orderdetails = new OrderDetail()
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                    };
                    _db.OrderDetails.Add(orderdetails);
                    // update stock here

                    var stock = await _db.Stocks.FirstOrDefaultAsync(a => a.BookId == item.BookId);
                    if (stock == null)
                    {
                        throw new InvalidOperationException("Stock is null");
                    }

                    if (item.Quantity > stock.Quantity)
                    {
                        throw new InvalidOperationException($"Only {stock.Quantity} items(s) are available in the stock");
                    }
                    // decrease the number of quantity from the stock table
                    stock.Quantity -= item.Quantity;
                }


                _db.CartDetails.RemoveRange(cartdetails);
                _db.SaveChanges();
                trancaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
