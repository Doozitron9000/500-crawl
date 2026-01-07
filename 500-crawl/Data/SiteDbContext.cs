using Microsoft.EntityFrameworkCore;
using _500_crawl.Authentication;
using _500_crawl.Models.Game;

public class SiteDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<GameState> Games => Set<GameState>();
    public SiteDbContext(DbContextOptions<SiteDbContext> options)
        : base(options)
    {
        
    }

    /// <summary>
    /// Override model creation so we don't get stuck with using sql style naming conventions in c#
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // call the super...... i don't think it actually does anything as its just a virtual method but lets just be safe..............
        base.OnModelCreating(modelBuilder);
        // convert our c# style user to conform to lower snake case
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.Username).HasColumnName("username");
            e.Property(u => u.PasswordHash).HasColumnName("password_hash");
            e.Property(u => u.GameId).HasColumnName("game_id");

            // make sure user is unique when we try and create one
            e.HasIndex(u => u.Username).IsUnique();
        });

        // do the same for game state
        modelBuilder.Entity<GameState>(e =>
        {
            e.ToTable("games");
            e.Property(g => g.Id).HasColumnName("id");
            e.Property(g => g.Phase).HasColumnName("phase");
            e.Property(g => g.PlayerHand).HasColumnName("player_hand");
            e.Property(g => g.AiHand).HasColumnName("ai_hand");
            e.Property(g => g.PlayerHealth).HasColumnName("player_health");
            e.Property(g => g.AiHealth).HasColumnName("ai_health");
            e.Property(g => g.WonHands).HasColumnName("won_hands");
            e.Property(g => g.LostHands).HasColumnName("lost_hands");
            e.Property(g => g.DeckSeed).HasColumnName("deck_seed");
            e.Property(g => g.DeckPlace).HasColumnName("deck_place");
            e.Property(g => g.RoundTarget).HasColumnName("round_target");
            e.Property(g => g.PlayerLeading).HasColumnName("player_leading");
            e.Property(g => g.Trumps).HasColumnName("trumps");
            e.Property(g => g.AiState).HasColumnName("ai_state");
            e.Property(g => g.AiCard).HasColumnName("ai_card");
        });
    }
}