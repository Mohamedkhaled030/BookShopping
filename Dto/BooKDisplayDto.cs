using bookShoop.Data;

namespace bookShoop.Dto
{
    public class BooKDisplayDto
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public string STerm = "";
        public int GenreId = 0;
    }
}
