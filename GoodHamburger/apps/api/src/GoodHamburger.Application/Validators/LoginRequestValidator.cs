using FluentValidation;
using GoodHamburger.Application.DTOs.Requests;

namespace GoodHamburger.Application.Validators;
public class LoginRequestValidator : AbstractValidator<LoginRequest> {

    public LoginRequestValidator() {

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
