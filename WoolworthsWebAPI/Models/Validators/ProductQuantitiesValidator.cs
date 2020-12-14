using FluentValidation;

namespace WoolworthsWebAPI.Models.Validators
{
    public class ProductQuantitiesValidator : AbstractValidator<ProductQuantities>
    {
        public ProductQuantitiesValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0);

        }
    }
}
