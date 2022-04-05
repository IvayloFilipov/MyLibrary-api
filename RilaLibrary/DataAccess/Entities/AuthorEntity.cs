using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

using static Common.GlobalConstants;

namespace DataAccess.Entities
{
    public class AuthorEntity : BaseDeletableModel<Guid>
    {
        [Required]
        [MaxLength(AUTHORNAME_MAX_LENGTH)]
        public string? AuthorName { get; set; }

        // Many-to-many - one author can have/write many books
        public ICollection<AuthorsBooks> AuthorsBooks { get; set; } = new HashSet<AuthorsBooks>();
    }
}
