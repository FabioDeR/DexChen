using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DexChen.Domain.Entities;

[Table("users")]
public class User : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("auth0_id")]
    public string Auth0Id { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;
}
