using Chefster.Models;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Context;

public class WeeklyNotesDbContext(DbContextOptions<WeeklyNotesDbContext> options) : DbContext(options)
{    
    public DbSet<WeeklyNotesModel> WeeklyNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder options)
    {
        // we can set restraints here like max attribute length, required or not, etc
        options.Entity<FamilyModel>().HasKey(e =>  e.Id).HasName("PK_weeklyNotes");
    }
}
