using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .WithMessage(EMAIL_REQUIRED_ERROR_MESSAGE)
                .Matches(USER_EMAIL_REGEX)
                .WithMessage(WRONG_EMAIL_ERROR_MESSAGE);

            RuleFor(u => u.Token)
                .NotEmpty();

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
        }
    }
}
