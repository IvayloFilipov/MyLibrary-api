using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public abstract class BaseModel<TKey> : IAuditableEntity
    {
        [Key]
        public TKey? Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
