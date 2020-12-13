using FluentValidation;

namespace WoolworthsWebAPI.Models.Validators
{
    public class SpecialsValidator : AbstractValidator<Special>
    {
        public SpecialsValidator()
        {
            RuleForEach(x => x.Quantities).SetValidator(new ProductQuantitiesValidator());
            RuleFor(x => x.Total)
                .ScalePrecision(2, 10)
                .GreaterThanOrEqualTo(0);
        }
    }
}
