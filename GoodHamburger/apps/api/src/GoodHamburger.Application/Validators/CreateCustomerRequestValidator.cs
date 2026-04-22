using FluentValidation;
using GoodHamburger.Application.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodHamburger.Application.Validators; 
public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest> {

    public CreateCustomerRequestValidator() {

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is requerido.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName  is requerido")
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone  is requerido.")
            .Matches(@"^\+?[0-9]{9,15}$")
            .WithMessage("Phone number should contain only digits. (9 a 15).");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format");
    }
}
