using Blazored.LocalStorage;
using DexChen.UI;
using DexChen.UI.Providers;
using DexChen.UI.Services;
using DexChen.UI.Services.Contract;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using Supabase;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// 🔹 Charger la config Supabase depuis appsettings.json
var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var config = await http.GetFromJsonAsync<Dictionary<string, string>>("appsettings.json");

var url = config["SUPABASE_URL"];
var key = config["SUPABASE_KEY"];
var pokemonKey = config["POKEMONCARDSAPI_KEY"];
// 🔹 HttpClient principal
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<PokemonApiClient>(options => new PokemonApiClient(pokemonKey));

// 🔹 LocalStorage (nécessaire pour la session Supabase)
builder.Services.AddBlazoredLocalStorage();




// ---------- BLAZOR AUTH

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddAuthorizationCore();

// 🔹 Enregistrer Supabase.Client avec SessionHandler
builder.Services.AddScoped<Client>(
    provider => new Client(
        url,
        key,
        new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false,
            SessionHandler = new CustomSupabaseSessionHandler(
                provider.GetRequiredService<ILocalStorageService>(),
                provider.GetRequiredService<ILogger<CustomSupabaseSessionHandler>>()
            )
        }
    )
);

// 🔹 Services et providers liés à l'authentification
builder.Services.AddScoped<IAuthService,AuthService>();

// 🔹 Services liés aux cartes Pokémon
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IScanService, ScanService>();

// ✅ Lancer l'application
await builder.Build().RunAsync();
