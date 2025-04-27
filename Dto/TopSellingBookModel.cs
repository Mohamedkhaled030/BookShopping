namespace BookShopping.Dto
{
    public class TopSellingBookModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public double Price { get; set; }
        public string? Image { get; set; }
        public int TotalSold { get; set; }
    }
}
