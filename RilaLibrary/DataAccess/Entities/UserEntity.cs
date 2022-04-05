using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

using static Common.GlobalConstants;

namespace DataAccess.Entities
{
    public class UserEntity : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(USER_LAST_NAME_MAX_LENGTH)]
        public string? LastName { get; set; }

        public bool IsApproved { get; set; }

        [ForeignKey(nameof(AddressEntity))]
        public Guid AddressEntityId { get; set; }

        public AddressEntity AddressesEntity { get; set; } = new AddressEntity();

        // Many-to-one - many comments can be made by one user (on one or several book)
        public ICollection<CommentEntity> CommentEntity { get; set; } = new HashSet<CommentEntity>();

        // Many-to-one - many books can be reserved by one user/reader
        public ICollection<BookReservationEntity> BookReservationEntity { get; set; } = new HashSet<BookReservationEntity>();

        // Many-to-one - one user can receive many notifications by librarians
        public ICollection<NotificationEntity> NotificationEntity { get; set; } = new HashSet<NotificationEntity>();
    }
}
