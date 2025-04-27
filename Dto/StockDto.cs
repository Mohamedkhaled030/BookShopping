using System.ComponentModel.DataAnnotations;

namespace BookShopping.Dto
{
    public class StockDto
    {
        public int BookId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Quntity Must Be a non-Negative Value")]
        public int quantity { get; set; }
    }
}
