using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class AuthorValidator: AbstractValidator<AuthorDto>
    {
        public AuthorValidator()
        {
            RuleFor(x => x.AuthorName)
                .NotEmpty()
                .WithMessage(AUTHORNAME_REQUIRED_ERROR_MESSAGE)
                .Length(AUTHORNAME_MIN_LENGTH, AUTHORNAME_MAX_LENGTH)
                .WithMessage(AUTHORNAME_LENGHT_ERROR_MESSAGE)
                .Matches(BOOK_ATTRIBUTE_REGEX)
                .WithMessage(AUTHORNAME_REGEX_ERROR_MESSAGE);
        }
    }
}
