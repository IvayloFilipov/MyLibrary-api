using System.ComponentModel.DataAnnotations;
using DataAccess.Models;

using static Common.GlobalConstants;

namespace DataAccess.Entities
{
    public class AddressEntity : BaseModel<Guid>
    {
        [Required]
        [MaxLength(ADDRESS_COUNTRY_NAME_MAX_LENGTH)]
        public string? Country { get; set; }

        [Required]
        [MaxLength(ADDRESS_CITY_NAME_MAX_LENGTH)]
        public string? City { get; set; }

        [Required]
        [MaxLength(ADDRESS_STREET_NAME_MAX_LENGTH)]
        public string? Street { get; set; }

        [Required]
        [MaxLength(ADDRESS_STREET_NUMBER_MAX_LENGTH)]
        public string? StreetNumber { get; set; }

        [MaxLength(ADDRESS_BUILDING_NUMBER_MAX_LENGTH)]
        public string? BuildingNumber { get; set; }

        [MaxLength(ADDRESS_APARTMENT_NUMBER_MAX_LENGTH)]
        public string? ApartmentNumber { get; set; }

        [MaxLength(ADDRESS_ADDITIONAL_INFO_MAX_LENGTH)]
        public string? AdditionalInfo { get; set; }

        // Many-to-one - many readers/users can live on one address
        public ICollection<UserEntity> UserEntity { get; set; } = new HashSet<UserEntity>();
    }
}
