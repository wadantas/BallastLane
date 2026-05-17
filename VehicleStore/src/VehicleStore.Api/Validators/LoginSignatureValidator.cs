using FluentValidation;
using VehicleStore.Api.Contracts.Signatures;

namespace VehicleStore.Api.Validators;

public class LoginSignatureValidator : AbstractValidator<LoginSignature>
{
    public LoginSignatureValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
