namespace Common.Models.InputDTOs
{
    public class Address
    {
        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string StreetNumber { get; set; } = string.Empty;

        public string? BuildingNumber { get; set; }

        public string? ApartmentNumber { get; set; }

        public string? AdditionalInfo { get; set; }
    }
}
