using Blazored.LocalStorage;
using DexChen.UI;
using DexChen.UI.Providers;
using DexChen.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Supabase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 🔹 HttpClient pour appels API ou fichier JSON
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// 🔹 LocalStorage pour la session Supabase
builder.Services.AddBlazoredLocalStorage();

// Replace the existing registration of Supabase.Client with the following code:
builder.Services.AddScoped<Supabase.Client>(provider =>
{
    var localStorage = provider.GetRequiredService<ILocalStorageService>();
    var logger = provider.GetRequiredService<ILogger<CustomSupabaseSessionHandler>>();

    var options = new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = false,
        SessionHandler = new CustomSupabaseSessionHandler(localStorage, logger)
    };

    var client = new Supabase.Client(
        "",
        "",
        options
    );
    client.InitializeAsync();
    return client;
});

// 🔹 Auth provider personnalisé
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(provider =>
    new CustomAuthStateProvider(
        provider.GetRequiredService<ILocalStorageService>(),
        provider.GetRequiredService<Supabase.Client>(),
        provider.GetRequiredService<ILogger<CustomAuthStateProvider>>()
    )
);

// 🔹 Auth + autorisation Blazor
builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthorizationCore();

// 🔹 Lancer l'app
await builder.Build().RunAsync();
