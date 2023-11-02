using Company.Services.Identity.Shared.ViewModels;
using FluentValidation;

namespace Company.WebApps.Identity.BlazorClient.Validations;

public class UserRoleValidator : AbstractValidator<UserRoleViewModel>
{
    public UserRoleValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MinimumLength(4);           
    }
}
