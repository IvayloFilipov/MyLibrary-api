using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class GenresBooks
    {
        [ForeignKey(nameof(GenreEntity))]
        public Guid GenreEntityId { get; set; }

        public GenreEntity GenresEntity { get; set; } = new GenreEntity();

        [ForeignKey(nameof(BookEntity))]
        public Guid BookEntityId { get; set; }

        public BookEntity BooksEntity { get; set; } = new BookEntity();
    }
}
