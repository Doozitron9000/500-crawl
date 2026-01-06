using Microsoft.EntityFrameworkCore;
using _500_crawl.Authentication;

public class SiteDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
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

            // make sure user is unique when we try and create one
            e.HasIndex(u => u.Username).IsUnique();
        });
    }
}