using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class GenreValidator : AbstractValidator<Genre>
    {
        public GenreValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                    .WithMessage(GENRE_NAME_REQUIRED_ERROR_MESSAGE)
                .MaximumLength(GENRE_NAME_MAX_LENGTH)
                    .WithMessage(GENRE_NAME_LENGTH_ERROR_MESSAGE)
                .Matches(BOOK_ATTRIBUTE_REGEX)
                    .WithMessage(GENRE_NAME_REGEX_ERROR_MESSAGE);
        }
    }
}
