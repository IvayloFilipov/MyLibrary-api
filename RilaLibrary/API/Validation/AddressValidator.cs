using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(a => a.Country)
                .NotEmpty()
                .WithMessage(COUNTRY_REQUIRED_ERROR_MESSAGE)
                .Length(ADDRESS_COUNTRY_NAME_MIN_LENGTH, ADDRESS_COUNTRY_NAME_MAX_LENGTH)
                .WithMessage(COUNTRY_LENGTH_NAME_ERROR_MESSAGE);

            RuleFor(a => a.City)
                .NotEmpty()
                .WithMessage(CITY_REQUIRED_ERROR_MESSAGE)
                .Length(ADDRESS_CITY_NAME_MIN_LENGTH, ADDRESS_CITY_NAME_MAX_LENGTH)
                .WithMessage(CITY_LENGTH_NAME_ERROR_MESSAGE);

            RuleFor(a => a.Street)
                .NotEmpty()
                .WithMessage(STREET_REQUIRED_ERROR_MESSAGE)
                .Length(ADDRESS_STREET_NAME_MIN_LENGTH, ADDRESS_STREET_NAME_MAX_LENGTH)
                .WithMessage(STREET_LENGTH_NAME_ERROR_MESSAGE);

            RuleFor(a => a.StreetNumber)
                .NotEmpty()
                .WithMessage(STREET_NUMBER_REQUIRED_ERROR_MESSAGE)
                .Length(ADDRESS_STREET_NUMBER_MIN_LENGTH, ADDRESS_STREET_NUMBER_MAX_LENGTH)
                .WithMessage(STREET_NUMBER_LENGTH_ERROR_MESSAGE);

            RuleFor(a => a.BuildingNumber)
                .MaximumLength(ADDRESS_BUILDING_NUMBER_MAX_LENGTH)
                .WithMessage(BUILDING_NUMBER_LENGTH_ERROR_MESSAGE);

            RuleFor(a => a.ApartmentNumber)
                .MaximumLength(ADDRESS_APARTMENT_NUMBER_MAX_LENGTH)
                .WithMessage(APARTMENT_NUMBER_LENGTH_ERROR_MESSAGE);

            RuleFor(a => a.AdditionalInfo)
                .MaximumLength(ADDRESS_ADDITIONAL_INFO_MAX_LENGTH)
                .WithMessage(ADDITIONAL_INFO_LENGTH_ERROR_MESSAGE);
        }
    }
}
