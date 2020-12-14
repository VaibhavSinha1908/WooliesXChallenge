using FluentValidation;

namespace WoolworthsWebAPI.Models.Validators
{
    public class SortOptionRequestValidator : AbstractValidator<SortOptionRequest>
    {
        public SortOptionRequestValidator()
        {
            RuleFor(x => x.SortOption)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}
