
using System.Security.Claims;
using Supabase.Gotrue;

namespace DexChen.UI.Services.Contract
{
    public interface IAuthService
    {
        Task ForgotPasswordAsync(string email);
        Task<List<Claim>> GetLoginClaimsAsync();
        Task<User?> GetUser();
        Task Logout();
        Task<bool> SignInAsync(string email, string password);
        Task<bool> SignUpAsync(string email, string password);
        Task<bool> UpdatePassword(string accessToken, string refreshToken, string newPassword);
    }
}
