using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public sealed class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator() => RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
}

public sealed class UpdateItemRequestValidator : AbstractValidator<UpdateItemRequest>
{
    public UpdateItemRequestValidator() => RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
}
