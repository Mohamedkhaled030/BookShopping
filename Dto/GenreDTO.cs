using System.ComponentModel.DataAnnotations;

namespace BookShopping.Dto
{
    public class GenreDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string GenreName { get; set; }
    }
}
