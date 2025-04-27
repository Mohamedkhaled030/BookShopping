using bookShoop.Data;

namespace BookShopping.Dto
{
    public class OrderDetailDTO
    {
        public string DivId { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }

    }
}
