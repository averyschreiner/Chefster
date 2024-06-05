using Chefster.Models;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Context;

public class MemberDbContext(DbContextOptions<MemberDbContext> options) : DbContext(options)
{    
    public DbSet<MemberModel> Members { get; set; }

    protected override void OnModelCreating(ModelBuilder options)
    {
        // we can set restraints here like max attribute length, required or not, etc
        options.Entity<MemberModel>().HasKey(e =>  e.FamilyId).HasName("PK_Members");
    }
}
