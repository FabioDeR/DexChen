using DexChen.Domain.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using Supabase.Gotrue;
using System.Text.Json;
namespace DexChen.UI.Services
{
    public class UserService
    {
        private readonly AuthenticationStateProvider _customAuthenticationStateProvider;

        public UserService(AuthenticationStateProvider customAuthenticationStateProvider)
        {
            _customAuthenticationStateProvider = customAuthenticationStateProvider;
        }

        public async Task<User?> GetUser()
        {

            throw new NotImplementedException("GetUser method is not implemented.");
        }
    }
}
