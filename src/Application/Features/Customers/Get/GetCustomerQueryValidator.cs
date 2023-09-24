using FluentValidation;
using FluentValidation.Results;

namespace Application.Features.Customers.Get
{
    public class GetCustomerQueryValidator : AbstractValidator<GetCustomerQuery>
    {
        public GetCustomerQueryValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .NotNull().NotEmpty().WithMessage("Id is required.");

        }
        protected override bool PreValidate(ValidationContext<GetCustomerQuery> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(nameof(GetCustomerQuery), "Provide a valid model."));
                return false;
            }
            return base.PreValidate(context, result);
        }
    }

}
