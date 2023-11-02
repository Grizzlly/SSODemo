using System.Threading.Tasks;

namespace Company.Services.Identity.Shared.Plugins;

/// <summary>
/// IEmailSender is used for sending messages through email
/// </summary>
public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
