using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DexChen.Domain.Entities;

[Table("favorites")]
public class Favorite : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("card_id")]
    public long CardId { get; set; }
}
