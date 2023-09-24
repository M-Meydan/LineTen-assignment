using FluentValidation;

namespace Application.Features.Customers.Create
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Phone)
            .Must(BeAValidPhoneNumber)
            .WithMessage("Invalid phone number.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }

        bool BeAValidPhoneNumber(int phoneNumber)
        {
            return phoneNumber >= 1000000000 && phoneNumber <= 9999999999;
        }
    }
}
