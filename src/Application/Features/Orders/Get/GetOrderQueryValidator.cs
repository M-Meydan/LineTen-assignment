using FluentValidation;
using FluentValidation.Results;

namespace Application.Features.Orders.Get
{
    public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
    {
        public GetOrderQueryValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .NotNull().NotEmpty().WithMessage("Id is required.");

        }
        protected override bool PreValidate(ValidationContext<GetOrderQuery> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(nameof(GetOrderQuery), "Provide a valid model."));
                return false;
            }
            return base.PreValidate(context, result);
        }
    }

}
