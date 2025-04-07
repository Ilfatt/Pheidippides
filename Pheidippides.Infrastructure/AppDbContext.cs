using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;

namespace Pheidippides.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Team> Teams { get; init; } = null!;
    public DbSet<Incident> Incidents { get; init; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne<Team>(x => x.Team)
            .WithMany(t => t.Workers)
            .HasForeignKey(x => x.TeamId);
        
        modelBuilder.Entity<User>()
            .HasOne<Team>(x => x.LeadTeam)
            .WithOne(t => t.Lead)
            .HasForeignKey<Team>(x => x.LeadId);

        modelBuilder.Entity<Team>()
            .HasIndex(x => x.InviteToken)
            .IsUnique();
    }
}