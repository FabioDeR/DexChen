using DexChen.UI.Services;
using DexChen.UI.Services.Contract;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace DexChen.UI.Providers;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ILogger<CustomAuthStateProvider> _logger;
    private readonly Supabase.Client _client;

    public CustomAuthStateProvider(IAuthService authService, ILogger<CustomAuthStateProvider> logger, Supabase.Client client)
    {
        _authService = authService;
        _logger = logger;
        _client = client;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            _logger.LogInformation("[AuthStateProvider] GetAuthenticationStateAsync");

            // Obtenez les claims à partir du service d'auth
            // Sets client auth and connects to realtime (if enabled)
            var claims = await _authService.GetLoginClaimsAsync();

            _logger.LogInformation("Claims obtenus : {Count}", claims.Count);

            var identity = claims.Count > 0
                ? new ClaimsIdentity(claims, "jwt", "email", "role")
                : new ClaimsIdentity();

            var principal = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(principal);

            // Notifiez les composants du changement d'état d'authentification
            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur dans AuthStateProvider");
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }


    public async Task ForceRefreshAsync()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
