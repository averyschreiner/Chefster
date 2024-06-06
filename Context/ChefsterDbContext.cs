using Chefster.Models;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Context;

public class ChefsterDbContext(DbContextOptions<ChefsterDbContext> options) : DbContext(options)
{
    // Define all of the tables we have
    public DbSet<FamilyModel> Families { get; set; }
    public DbSet<MemberModel> Members { get; set; }
    public DbSet<WeeklyNotesModel> WeeklyNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder options)
    {
        // define what model goes to what table
        options.Entity<FamilyModel>().ToTable("Families");
        options.Entity<MemberModel>().ToTable("Members");
        options.Entity<WeeklyNotesModel>().ToTable("WeeklyNotes");
    }
}
