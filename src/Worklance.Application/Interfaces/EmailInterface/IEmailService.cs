using System.Threading.Tasks;

namespace Worklance.Application.Interfaces.EmailInterface
{
    public interface IEmailService
    {
        Task SendOtpAsync(string email, string otp);
    }
}
