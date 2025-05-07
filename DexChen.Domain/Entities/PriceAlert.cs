using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DexChen.Domain.Entities;

[Table("price_alerts")]
public class PriceAlert : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("card_id")]
    public long CardId { get; set; }

    [Column("target_price")]
    public decimal TargetPrice { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
