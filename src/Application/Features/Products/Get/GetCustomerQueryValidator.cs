using FluentValidation;
using FluentValidation.Results;

namespace Application.Features.Products.Get
{
    public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
    {
        public GetProductQueryValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .NotNull().NotEmpty().WithMessage("Id is required.");

        }
        protected override bool PreValidate(ValidationContext<GetProductQuery> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(nameof(GetProductQuery), "Provide a valid model."));
                return false;
            }
            return base.PreValidate(context, result);
        }
    }

}
