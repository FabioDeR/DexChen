using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Supabase.Gotrue;
using System.Security.Claims;
using System.Text.Json;
using DexChen.UI.Services.Contract;

namespace DexChen.UI.Services;

public class AuthService : IAuthService
{
    private readonly Supabase.Client _client;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthService> _logger;
    private readonly NavigationManager _navigation;

    public AuthService(
        Supabase.Client client,
        ILocalStorageService localStorage,
        ILogger<AuthService> logger,
        NavigationManager navigation)
    {
        _client = client;
        _localStorage = localStorage;
        _logger = logger;
        _navigation = navigation;
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignIn(email, password);

            if (session?.User == null)
            {
                _logger.LogWarning("La connexion a réussi mais l'utilisateur est null");
                return false;
            }

            _logger.LogInformation("Connexion réussie pour {Email}", email);

            // Vérifiez si la session est correctement stockée
            var storedSession = await _localStorage.GetItemAsync<Session>("SUPABASE_SESSION");
            _logger.LogInformation("Session stockée dans localStorage: {IsStored}", storedSession != null);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion");
            return false;
        }
    }

    public async Task<bool> SignUpAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignUp(email, password);
            return session?.User != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription");
            return false;
        }
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var redirectUrl = $"{_navigation.BaseUri}reset-password";
        var options = new ResetPasswordForEmailOptions(email)
        {
            RedirectTo = redirectUrl
        };

        await _client.Auth.ResetPasswordForEmail(options);
    }

    public async Task<bool> UpdatePassword(string accessToken, string refreshToken, string newPassword)
    {
        try
        {
            await _client.Auth.SetSession(accessToken, refreshToken);

            var user = await _client.Auth.Update(new UserAttributes
            {
                Password = newPassword
            });

            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du changement de mot de passe");
            return false;
        }
    }

    public async Task Logout()
    {
        await _client.Auth.SignOut();
        await _localStorage.RemoveItemAsync("SUPABASE_SESSION");
        _navigation.NavigateTo("/", forceLoad: true);
    }

    public async Task<User?> GetUser()
    {
        var session = await _client.Auth.RetrieveSessionAsync();
        return session?.User;
    }

    public async Task<List<Claim>> GetLoginClaimsAsync()
    {
        try
        {
            var session = _client.Auth.CurrentSession;

            if (session == null)
            {
                _logger.LogInformation("Tentative de récupération de session depuis Supabase");
                try
                {
                    session = await _localStorage.GetItemAsync<Session>("SUPABASE_SESSION");
                    //session = await _client.Auth.RetrieveSessionAsync();
                    _logger.LogInformation("Session récupérée avec succès : {UserEmail}", session?.User?.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Échec de récupération de session depuis Supabase");
                }
            }
            else
            {
                _logger.LogInformation("Session existante trouvée : {UserEmail}", session.User?.Email);
            }

            if (session?.AccessToken == null)
            {
                _logger.LogWarning("Aucun access token trouvé dans la session");
                return new List<Claim>();
            }

            var claims = ParseAccessToken(session.AccessToken);

            // Ajoutez des claims supplémentaires pour l'utilisateur
            if (session.User != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, session.User.Email ?? string.Empty));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, session.User.Id));
            }

            return claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des claims");
            return new List<Claim>();
        }
    }

    private List<Claim> ParseAccessToken(string accessToken)
    {
        try
        {
            var parts = accessToken.Split('.');
            if (parts.Length != 3)
            {
                _logger.LogWarning("Format JWT invalide");
                return new List<Claim>();
            }

            var payload = parts[1];
            var json = ParseBase64WithoutPadding(payload);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return dict?
                .Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty))
                .ToList() ?? new List<Claim>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du décodage du token JWT");
            return new List<Claim>();
        }
    }


    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
            case 1: base64 += "==="; break;
        }

        return Convert.FromBase64String(base64);
    }

}
