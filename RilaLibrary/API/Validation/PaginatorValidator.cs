using FluentValidation;
using Common.Models.InputDTOs;

using static Common.GlobalConstants;

namespace API.Validation
{
    public class PaginatorValidator : AbstractValidator<PaginatorInputDto>
    {
        public PaginatorValidator()
        {
            RuleFor(x => x.Page)
                .NotEmpty()
                    .WithMessage(PAGE_NUMBER_REQUIRED_ERROR_MESSAGE)
                .GreaterThanOrEqualTo(1)
                    .WithMessage(PAGE_NUMBER_ATLEAST_ONE_ERROR_MESSAGE);

            RuleFor(x => x.PageSize)
                .NotEmpty()
                    .WithMessage(PAGE_SIZE_REQUIRED_ERROR_MESSAGE)
                .GreaterThanOrEqualTo(1)
                    .WithMessage(PAGE_SIZE_ATLEAST_ONE_ERROR_MESSAGE);
        }
    }
}
