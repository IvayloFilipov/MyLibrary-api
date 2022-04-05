using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Models;

namespace DataAccess.Entities
{
    public class BookReservationEntity : BaseDeletableModel<Guid>
    {
        public bool IsApproved { get; set; }

        public bool IsReviewed { get; set; }

        public DateTime ReceiveDate { get; set; }

        public DateTime ReturnDate { get; set; }

        [ForeignKey(nameof(UserEntity))]
        public Guid UserEntityId { get; set; }

        [ForeignKey(nameof(BookEntity))]
        public Guid BookEntityId { get; set; }

        [ForeignKey(nameof(UserEntity))]
        public Guid LibrarianId { get; set; }
    }
}
