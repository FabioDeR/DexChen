using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DexChen.Domain.Entities;

[Table("cards")]
public class CardDexChen : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("series")]
    public string Series { get; set; } = string.Empty;

    [Column("number")]
    public string Number { get; set; } = string.Empty;

    [Column("rarity")]
    public string? Rarity { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }
}
