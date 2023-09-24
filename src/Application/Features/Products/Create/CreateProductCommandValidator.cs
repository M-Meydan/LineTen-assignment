using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Features.Products.Create
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.SKU).GreaterThan(0);
        }
    }
}
