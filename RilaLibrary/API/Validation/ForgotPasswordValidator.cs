using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .WithMessage(EMAIL_REQUIRED_ERROR_MESSAGE)
                .Matches(USER_EMAIL_REGEX)
                .WithMessage(WRONG_EMAIL_ERROR_MESSAGE);
        }
    }
}
