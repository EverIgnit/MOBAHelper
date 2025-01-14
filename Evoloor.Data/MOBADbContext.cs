using Microsoft.EntityFrameworkCore;
using Evoloor.MOBACore;
namespace Evoloor.Data;
public abstract class MOBADbContext<TMatch, TMatchPlayer, TAccount, TChampion, TItem> : DbContext
    where TChampion : class, IStaticData
    where TAccount : class, IAccount
    where TItem : class, IStaticData
    where TMatch : class, IMatch<TMatchPlayer, TAccount, TChampion, TItem>
    where TMatchPlayer : class, IMatchPlayer<TAccount, TChampion, TItem>
{
    protected readonly int SHORT_STR_LENGTH = 64;
    public DbSet<TChampion> Champions { get; set; }
    public DbSet<TItem> Items { get; set; }
    public DbSet<TMatch> Matches { get; set; }
    public DbSet<TMatchPlayer> MatchPlayers { get; set; }
    public DbSet<MatchPerspective<TMatch, TMatchPlayer, TAccount, TChampion, TItem>> MatchPerspectives { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IStaticData>()
            .HasKey(e => e.Name);
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(entityType=>
                entityType.GetProperties()
                .Where(property=>property.ClrType == typeof(string))))
            property.SetMaxLength(SHORT_STR_LENGTH);
        base.OnModelCreating(modelBuilder);
    }
}
