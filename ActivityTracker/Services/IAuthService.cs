using ActivityTracker.Models;

namespace ActivityTracker.Services
{
    public interface IAuthService
    {
        Task<(int, string)> Registration(RegistrationModel model, string role);
        Task<(int, string, string, string)> Login(LoginModel model);
    }
}
