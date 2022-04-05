using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

using static Common.GlobalConstants;

namespace DataAccess.Entities
{
    public class BookEntity : BaseDeletableModel<Guid>
    {
        [Required]
        [MaxLength(BOOK_TITLE_MAX_LENGTH)]
        public string? Title { get; set; }

        [MaxLength(BOOK_DESCRIPTION_MAX_LENGTH)]
        public string? Description { get; set; }

        public string? ImageAddress { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public Guid Sku { get; set; }

        [Range(BOOK_RANGE_MIN_VALUE, BOOK_RANGE_MAX_VALUE)]
        public int TotalQuantity { get; set; }

        public int CurrentQuantity { get; set; }

        public int BorrowedTime { get; set; } = BOOK_STANDARD_BORROW_PERIOD;
        // Many-to-many - one book can have many authors

        public ICollection<AuthorsBooks> AuthorsBooks { get; set; } = new HashSet<AuthorsBooks>();

        // Many-to-many - one book can be in several genres
        public ICollection<GenresBooks> GenresBooks { get; set; } = new HashSet<GenresBooks>();

        // One-to-many - one book can have many comments
        public ICollection<CommentEntity> CommentEntity { get; set; } = new HashSet<CommentEntity>();

        // One-to-many - one book can have many reservation requests (if we have more than one SKU)
        public ICollection<BookReservationEntity> BookReservationEntity { get; set; } = new HashSet<BookReservationEntity>();

        // One-to-many - one book can have many prolonging requests
        public ICollection<ProlongingRequestEntity> ProlongingRequestEntity { get; set; } = new HashSet<ProlongingRequestEntity>();
    }
}
