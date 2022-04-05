using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class LoginUserValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .WithMessage(EMAIL_REQUIRED_ERROR_MESSAGE)
                .Matches(USER_EMAIL_REGEX)
                .WithMessage(WRONG_EMAIL_ERROR_MESSAGE);

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage(PASSWORD_REQUIRED_ERROR_MESSAGE);
        }
    }
}
