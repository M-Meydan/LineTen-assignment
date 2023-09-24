using FluentValidation;
using System.Text.RegularExpressions;

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
            int minPhoneNumber = 0;
            int maxPhoneNumber = int.MaxValue;

            return phoneNumber >= minPhoneNumber && phoneNumber <= maxPhoneNumber;
        }
    }
}
