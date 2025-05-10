using DexChen.Domain.Dtos;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace DexChen.UI.Services.Contract;

public interface ICardService
{
    Task<List<Card>> GetFeaturedCardsAsync(int count = 5);
    Task<Card?> GetCardByIdAsync(string id);
    Task<List<Card>> SearchCardsByNameAsync(string name);
    Task<List<PokemonCard>> SearchCardsByScanResultAsync(ScanResult result);
    PokemonCard? PickBestMatch(ScanResult scan, List<PokemonCard> cards);
    Task<List<PokemonCard>> SearchWithFallbackTermsAsync(ScanResult result);
}
