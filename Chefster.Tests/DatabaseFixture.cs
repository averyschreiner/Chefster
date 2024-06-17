using Chefster.Context;
using Chefster.Models;
using Chefster.Services;
using Microsoft.EntityFrameworkCore;

namespace Chefster.Tests;

public class DatabaseFixture
{
    public ChefsterDbContext Context { get; private set; }
    public FamilyService FamilyService { get; private set; }
    public MemberService MemberService { get; private set; }
    public ConsiderationsService ConsiderationsService { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ChefsterDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB" + Guid.NewGuid().ToString())
            .Options;

        Context = new ChefsterDbContext(options);

        FamilyService = new FamilyService(Context);
        MemberService = new MemberService(Context, FamilyService);
        ConsiderationsService = new ConsiderationsService(Context);
    }

    // Add items to database for testing
    public void Initialize()
    {

        Context.Families.AddRange(
            new FamilyModel
            {
                Id = "1",
                CreatedAt = DateTime.Now,
                Email = "test@email.com",
                FamilySize = 2,
                GenerationDay = DayOfWeek.Friday,
                GenerationTime = new TimeSpan(10000),
                PhoneNumber = "0001112222"
            },
            new FamilyModel
            {
                Id = "4",
                CreatedAt = DateTime.Now,
                Email = "test4@email.com",
                FamilySize = 8,
                GenerationDay = DayOfWeek.Monday,
                GenerationTime = new TimeSpan(19000),
                PhoneNumber = "5556664444"
            }
        );

        Context.Members.AddRange(
            new MemberModel
            {
                MemberId = "mem1",
                FamilyId = "1",
                Name = "testName"
            },
            new MemberModel
            {
                MemberId = "mem2",
                FamilyId = "1",
                Name = "testName2"
            },
            new MemberModel
            {
                MemberId = "mem3",
                FamilyId = "1",
                Name = "testName3"
            }
        );
        Context.SaveChanges();
    }

    // cleanup db after test run. Runs automatically
    public void Cleanup()
    {
        Context.Database.EnsureDeleted();
        foreach (var entity in Context.ChangeTracker.Entries().ToList())
        {
            entity.State = EntityState.Detached;
        }
    }
}
