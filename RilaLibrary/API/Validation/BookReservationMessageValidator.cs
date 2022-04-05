using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class BookReservationMessageValidator : AbstractValidator<BookReservationMessageDto>
    {
        public BookReservationMessageValidator()
        {
            RuleFor(u => u.Message)
                .NotEmpty()
                .WithMessage(MESSAGE_REQUIRED_ERROR_MESSAGE)
                .MaximumLength(MESSAGE_MAX_LENGTH)
                .WithMessage(MESSAGE_LENGTH_ERROR_MESSAGE);
        }
    }
}
