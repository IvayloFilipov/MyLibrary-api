using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .WithMessage(FIRST_NAME_REQUIRED_ERROR_MESSAGE)
                .Length(USER_FIRST_NAME_MIN_LENGTH, USER_FIRST_NAME_MAX_LENGTH)
                .WithMessage(FIRST_NAME_LENGTH_ERROR_MESSAGE);

            RuleFor(u => u.LastName)
                .NotEmpty()
                .WithMessage(LAST_NAME_REQUIRED_ERROR_MESSAGE)
                .Length(USER_LAST_NAME_MIN_LENGTH,USER_LAST_NAME_MAX_LENGTH)
                .WithMessage(LAST_NAME_LENGTH_ERROR_MESSAGE);

            RuleFor(u => u.Email)
                .NotEmpty()
                .WithMessage(EMAIL_REQUIRED_ERROR_MESSAGE)
                .Matches(USER_EMAIL_REGEX)
                .WithMessage(WRONG_EMAIL_ERROR_MESSAGE);

            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .WithMessage(PHONE_NUMBER_REQUIRED_ERROR_MESSAGE)
                .Matches(USER_PHONE_NUMBER_REGEX)
                .WithMessage(INVALID_PHONE_NUMBER_ERROR_MESSAGE);

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage(PASSWORD_REQUIRED_ERROR_MESSAGE)
                .Length(USER_PASSWORD_MIN_LENGTH, USER_PASSWORD_MAX_LENGTH)
                .Matches(USER_PASSWORD_REGEX)
                .WithMessage(WRONG_PASSWORD_ERROR_MESSAGE);

            RuleFor(u => u.ConfirmPassword)
                .NotEmpty()
                .WithMessage(CONFIRM_PASSWORD_REQUIRED_ERROR_MESSAGE)
                .Equal(u => u.Password)
                .WithMessage(COMPARE_PASSWORDS_ERROR_MESSAGE);

            RuleFor(u => u.Address)
                .SetValidator(new AddressValidator());
        }
    }
}
