using DexChen.UI.Providers;
using DexChen.UI.Services.Contract;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace DexChen.UI.Pages
{
    public partial class Login
    {
        [Inject] IAuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;

        private string email = "";
        private string password = "";
        private string errorMessage = "";
        private bool showPassword = false;

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }
        private async Task HandleLogin()
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Veuillez remplir tous les champs.";
                return;
            }

            try
            {
                await AuthService.SignInAsync(email, password); // Removed assignment to 'success' as SignInAsync returns void
                if (AuthProvider is CustomAuthStateProvider provider)
                {
                    await provider.GetAuthenticationStateAsync(); // met à jour les claims
                }

                Navigation.NavigateTo("/");
            }
            catch (Exception ex)
            {
                errorMessage = "Erreur lors de la connexion. Veuillez réessayer.";
            }
        }
    }
}