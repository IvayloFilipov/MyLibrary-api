using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class AuthorsBooks
    {
        [ForeignKey(nameof(AuthorEntity))]
        public Guid AuthorEntityId { get; set; }

        public AuthorEntity AuthorsEntity { get; set; } = new AuthorEntity();

        [ForeignKey(nameof(BookEntity))]
        public Guid BookEntityId { get; set; }

       public BookEntity BooksEntity { get; set; } = new BookEntity();
    }
}
