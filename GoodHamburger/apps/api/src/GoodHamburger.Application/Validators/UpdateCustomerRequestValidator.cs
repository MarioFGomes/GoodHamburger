using FluentValidation;
using GoodHamburger.Application.DTOs.Requests;

namespace GoodHamburger.Application.Validators;
public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest> {
    public UpdateCustomerRequestValidator() {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[0-9]{9,15}$");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
