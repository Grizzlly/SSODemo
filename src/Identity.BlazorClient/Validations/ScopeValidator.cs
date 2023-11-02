using Company.Services.Identity.Shared.ViewModels;
using FluentValidation;

namespace Company.WebApps.Identity.BlazorClient.Validations;

public class ScopeValidator : AbstractValidator<ScopeViewModel>
{
    public ScopeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(4);
        RuleFor(x => x.DisplayName).NotEmpty().MinimumLength(4);          
    }
}
