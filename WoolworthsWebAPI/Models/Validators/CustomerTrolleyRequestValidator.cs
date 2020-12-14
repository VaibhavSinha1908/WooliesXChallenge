using FluentValidation;

namespace WoolworthsWebAPI.Models.Validators
{
    public class CustomerTrolleyRequestValidator : AbstractValidator<CustomerTrolleyRequest>
    {
        public CustomerTrolleyRequestValidator()
        {
            RuleFor(x => x.Products)
                .NotEmpty().WithMessage("The request has to have atleast 1 product.");
            RuleFor(x => x.Quantities)
                .NotEmpty().WithMessage("The request has to have atleast 1 quantity for product.");
        }
    }
}
