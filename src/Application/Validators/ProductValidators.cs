using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator() => RuleFor(x => x.ProductName).NotEmpty().MaximumLength(255);
}

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator() => RuleFor(x => x.ProductName).NotEmpty().MaximumLength(255);
}
