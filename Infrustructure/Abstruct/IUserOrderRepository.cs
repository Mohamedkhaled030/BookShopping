using bookShoop.Data;
using BookShopping.Dto;

namespace BookShopping.Infrustructure.Abstruct
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> GetAllUserOrders(bool getAll = false);
        Task ChangeOrderStatus(UpdateOrderStatusDto data);
        Task TogglePaymentStatus(int orderId);
        Task<Order?> GetOrderById(int id);
        Task<IEnumerable<OrderStatus>> GetOrderStatuses();
    }
}
