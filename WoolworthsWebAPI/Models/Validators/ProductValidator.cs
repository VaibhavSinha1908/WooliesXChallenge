using FluentValidation;

namespace WoolworthsWebAPI.Models.Validators
{
    public class TrolleyProductValidator : AbstractValidator<TrolleyProduct>
    {
        public TrolleyProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Price)
                .ScalePrecision(2, 10)
                .GreaterThanOrEqualTo(0);

        }
    }
}
