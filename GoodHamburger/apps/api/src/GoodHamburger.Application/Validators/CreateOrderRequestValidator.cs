using FluentValidation;
using GoodHamburger.Application.DTOs.Requests;

namespace GoodHamburger.Application.Validators;
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest> {

    public CreateOrderRequestValidator() {

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage("MenuId is required.");

        // "sideDishIds": null in the payload overwrites the DTO default with
        // null, so the rule must tolerate it instead of throwing a 500.
        RuleFor(x => x.SideDishIds)
            .Must(ids => ids is null || ids.Count <= 2)
            .WithMessage("An order can have at most 2 side dishes (one FRIES and one DRINK).");
    }
}
