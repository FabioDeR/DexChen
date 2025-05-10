using System.Net;
using DexChen.Domain.Dtos;
using DexChen.UI.Services.Contract;
using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace DexChen.UI.Services
{
    public class CardService : ICardService, IDisposable
    {
        private readonly PokemonApiClient _pokemonClient;
        private bool _disposed = false;
        private const string API_KEY = ""; // Remplacez par votre clé API réelle

        public CardService(HttpClient httpClient)
        {
            // Configurer le HttpClient avec l'en-tête d'API key
            if (!httpClient.DefaultRequestHeaders.Contains("X-Api-Key"))
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", API_KEY);
            }

            // S'assurer que l'URL de base est correctement définie
            if (httpClient.BaseAddress == null)
            {
                httpClient.BaseAddress = new Uri("https://api.pokemontcg.io/v2/");
            }

            // Créer le client Pokemon TCG
            _pokemonClient = new PokemonApiClient(httpClient);
        }

        public async Task<List<Card>> GetFeaturedCardsAsync(int count = 5)
        {
            try
            {
                // Utiliser les filtres pour obtenir des cartes rares
                var filters = new Dictionary<string, string>
                {
                    { "rarity", "Rare Holo|Rare Ultra|Rare Holo GX" }
                };

                // Utiliser l'API client pour récupérer les ressources
                var result = await _pokemonClient.GetApiResourceAsync<Card>(count, 0, filters);
                var result2 = PokemonFilterBuilder.CreatePokemonFilter();
               return result?.Results?.ToList() ?? new List<Card>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur lors de la récupération des cartes: {ex.Message}");
                return new List<Card>();
            }
        }

        public async Task<Card?> GetCardByIdAsync(string id)
        {
            try
            {
                var filters = new Dictionary<string, string> { { "id", id } };
                var result = await _pokemonClient.GetApiResourceAsync<Card>(1, 0, filters);

                return result?.Results?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur lors de la récupération de la carte {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<PokemonCard>> SearchCardsByScanResultAsync(ScanResult result)
        {
            try
            {
                var filter = PokemonFilterBuilder.CreatePokemonFilter();

                if (!string.IsNullOrWhiteSpace(result.EnglishName))
                    filter.AddName(result.EnglishName);

                //if (!string.IsNullOrWhiteSpace(result.CardNumber) && result.CardNumber != "Unknown")
                //{
                //    var extracted = ExtractNumber("1");
                //    if (!string.IsNullOrWhiteSpace(extracted))
                //        filter.Add("number", $"\"{extracted}\"");
                //}

                if (!string.IsNullOrWhiteSpace(result.EnglishSetName) && result.EnglishSetName.ToLower() != "null")
                    filter.AddSetName(result.EnglishSetName);

                //if (!string.IsNullOrWhiteSpace(result.EnglishRarity) && result.EnglishRarity.ToLower() != "unknown")
                //    filter.AddRarity(result.EnglishRarity);

                var resource = await _pokemonClient.GetApiResourceAsync<PokemonCard>(filter);
                return resource?.Results?.ToList() ?? new List<PokemonCard>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur filtre Pokémon TCG : " + ex.Message);
                return new List<PokemonCard>();
            }
        }
        private string ExtractNumber(string cardNumber)
        {
            return cardNumber.Contains('/') ? cardNumber.Split('/')[0] : cardNumber;
        }

        public PokemonCard? PickBestMatch(ScanResult scan, List<PokemonCard> cards)
        {
            return cards
                .Select(card =>
                {
                    int score = 0;

                    if (string.Equals(card.Number, scan.CardNumber, StringComparison.OrdinalIgnoreCase))
                        score += 4;

                    if (string.Equals(card.Set?.Id, scan.SetCode, StringComparison.OrdinalIgnoreCase))
                        score += 3;

                    if (string.Equals(card.Name, scan.EnglishName, StringComparison.OrdinalIgnoreCase))
                        score += 2;

                    if (!string.IsNullOrEmpty(scan.EnglishRarity) &&
                        string.Equals(card.Rarity, scan.EnglishRarity, StringComparison.OrdinalIgnoreCase))
                        score += 1;

                    if (!string.IsNullOrEmpty(scan.EnglishIllustrator) &&
                        string.Equals(card.Artist, scan.EnglishIllustrator, StringComparison.OrdinalIgnoreCase))
                        score += 1;

                    return new { Card = card, Score = score };
                })
                .OrderByDescending(c => c.Score)
                .ThenBy(c => c.Card.Name) // tie-break
                .Select(c => c.Card)
                .FirstOrDefault();
        }


        public async Task<List<PokemonCard>> SearchWithFallbackTermsAsync(ScanResult result)
        {
            foreach (var term in result.SuggestedSearchTerms.Distinct())
            {
                var filter = PokemonFilterBuilder.CreatePokemonFilter();

                // Essayer avec nom
                filter.AddName(term);

                var resource = await _pokemonClient.GetApiResourceAsync<PokemonCard>(filter);
                var cards = resource?.Results?.ToList();

                if (cards?.Any() == true)
                    return cards;

                // BONUS : essai libre sur n° ou set (si le nom échoue)
                var fallbackFilter = PokemonFilterBuilder.CreatePokemonFilter();
                fallbackFilter.Add("q", term); // dépend du moteur de recherche sous-jacent

                var fallbackResource = await _pokemonClient.GetApiResourceAsync<PokemonCard>(fallbackFilter);
                var fallbackCards = fallbackResource?.Results?.ToList();

                if (fallbackCards?.Any() == true)
                    return fallbackCards;
            }

            return new List<PokemonCard>();
        }




        public async Task<List<Card>> SearchCardsByNameAsync(string name)
        {
            try
            {
                // Créer un filtre pour rechercher par nom
                var filters = new Dictionary<string, string> { { "name", name } };
                var result = await _pokemonClient.GetApiResourceAsync<Card>(20, 0, filters);

                return result?.Results?.ToList() ?? new List<Card>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur lors de la recherche de cartes avec le nom {name}: {ex.Message}");
                return new List<Card>();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _pokemonClient.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
