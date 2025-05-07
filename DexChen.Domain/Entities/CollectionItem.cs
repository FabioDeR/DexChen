using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DexChen.Domain.Entities;

[Table("collection_items")]
public class CollectionItem : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("card_id")]
    public long CardId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("condition")]
    public string Condition { get; set; } = string.Empty;

    [Column("date_added")]
    public DateTime DateAdded { get; set; }
}
