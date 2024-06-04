using Catalog.Core.Entities;
using FluentValidation;

namespace Catalog.Core.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name).NotEmpty().Length(0, 100);
        RuleFor(p => p.Description).MaximumLength(255);
        RuleFor(p => p.Price).GreaterThan(0);
        RuleFor(p => p.Stock).GreaterThanOrEqualTo(0);
    }
}