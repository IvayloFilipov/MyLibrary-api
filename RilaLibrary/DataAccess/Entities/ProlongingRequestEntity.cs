using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Models;

using static Common.GlobalConstants;

namespace DataAccess.Entities
{
    public class ProlongingRequestEntity : BaseDeletableModel<Guid>
    {
        public bool IsAproved { get; set; }

        [MaxLength(PROLONGING_DESCRIPTION_MAX_LENGTH)]
        public string? Description { get; set; }

        [Range(PROLONGING_RANGE_MIN_VALUE, PROLONGING_RANGE_MAX_VALUE)]
        public int ProlongingDays { get; set; }

        [ForeignKey(nameof(UserEntity))]
        public Guid UserEntityId { get; set; }

        [ForeignKey(nameof(BookEntity))]
        public Guid BookEntityId { get; set; }

        [ForeignKey(nameof(UserEntity))]
        public Guid LibrarianId { get; set; }
    }
}
