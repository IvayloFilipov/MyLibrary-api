using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Enums;
using DataAccess.Models;

using static Common.GlobalConstants;

namespace DataAccess.Entities
{
    public class CommentEntity : BaseModel<Guid>
    {
        [Required]
        [MaxLength(COMMENT_DESCRIPTION_MAX_LENGTH)]
        public string? Description { get; set; }

        public bool IsVisible { get; set; }

        // Enum
        public Rating Rating { get; set; }

        [ForeignKey(nameof(UserEntity))]
        public Guid UserEntityId { get; set; }

        public UserEntity UsersEntity { get; set; } = new UserEntity();
    }
}
