using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class AddBookValidator : AbstractValidator<AddBookDto>
    {
        public AddBookValidator()
        {
            RuleFor(a => a.BookTitle)
               .NotEmpty()
               .WithMessage(BOOKTITLE_REQUIRED_ERROR_MESSAGE)
               .Length(BOOK_TITLE_MIN_LENGTH,BOOK_TITLE_MAX_LENGTH)
               .WithMessage(BOOKTITLE_LENGTH_ERROR_MESSAGE)
               .Matches(BOOK_ATTRIBUTE_REGEX)
               .WithMessage(BOOKTITLE_REGEX_ERROR_MESSAGE);

            RuleFor(a => a.Description)
              .MaximumLength(BOOK_DESCRIPTION_MAX_LENGTH)
              .WithMessage(DESCRIPTION_LENGTH_ERROR_MESSAGE);
            
            RuleFor(a => a.BookAuthors)
              .NotEmpty()
              .WithMessage(AUTHOR_REQUIRED_ERROR_MESSAGE);

            RuleForEach(a => a.Genres)
             .NotEmpty()
             .WithMessage(GENRE_REQUIRED_ERROR_MESSAGE);
        }
    }
}

