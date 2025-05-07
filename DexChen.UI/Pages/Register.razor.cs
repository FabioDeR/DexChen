using DexChen.UI.Providers;
using DexChen.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace DexChen.UI.Pages
{
    public partial class Register
    {
        private string email = "";
        private string password = "";
        private string confirmPassword = "";
        private string errorMessage = "";
        private bool showPassword = false;

        
        [Inject] private AuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }
        private async Task HandleRegister()
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Veuillez remplir tous les champs.";
                return;
            }

            if (password != confirmPassword)
            {
                errorMessage = "Les mots de passe ne correspondent pas.";
                return;
            }

            try
            {
                var success = await AuthService.SignUpAsync(email, password);
                if (!success)
                {
                    errorMessage = "Impossible de créer le compte. L'email est peut-être déjà utilisé.";
                    return;
                }

                if (AuthProvider is CustomAuthStateProvider provider)
                {
                    await provider.GetAuthenticationStateAsync(); // déclenche aussi Notify
                }

                Navigation.NavigateTo("/");
            }
            catch (Exception ex)
            {
                errorMessage = "Erreur lors de l'inscription. Veuillez réessayer.";
            }
        }
    }
}