using bookShoop.Data;
using BookShopping.Dto;

namespace BookShopping.Infrustructure.Abstruct
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);
        Task<object> GetUserCart();
        Task<int> GetCartItemCount(string userid = "");
        Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckout(CheckoutDto checkoutDto);



    }
}
