using FluentValidation;
using GoodHamburger.Application.DTOs.Requests;

namespace GoodHamburger.Application.Validators;
public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest> {

    public CreateMenuRequestValidator() {

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Invalid currency value.");
    }
}
