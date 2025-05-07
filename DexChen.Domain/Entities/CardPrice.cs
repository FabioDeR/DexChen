using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DexChen.Domain.Entities;

[Table("card_prices")]
public class CardPrice : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("card_id")]
    public long CardId { get; set; }

    // Supabase ne gère pas les relations automatiques, on ne met pas la navigation `Card` ici

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("price_min")]
    public decimal PriceMin { get; set; }

    [Column("price_avg")]
    public decimal PriceAvg { get; set; }

    [Column("price_max")]
    public decimal PriceMax { get; set; }
}
