using BankingSuite.BuildingBlocks.Domain.Entities;
using BankingSuite.BuildingBlocks.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BankingSuite.IamService.UnitTests.Infrastructure;

public class RepositoryBaseTests
{
    [Fact]
    public async Task Repository_Should_Add_Get_And_Remove_Aggregate()
    {
        var options = new DbContextOptionsBuilder<FakeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new FakeDbContext(options);
        var repo = new DummyRepository(db);

        var entity = new DummyAggregate(Guid.NewGuid());

        await repo.AddAsync(entity);
        await db.SaveChangesAsync();

        var fetched = await repo.GetByIdAsync(entity.Id);
        fetched.Should().NotBeNull();

        repo.Remove(entity);
        await db.SaveChangesAsync();

        (await repo.GetByIdAsync(entity.Id)).Should().BeNull();
    }

    private sealed class FakeDbContext(DbContextOptions<FakeDbContext> options)
        : DbContext(options)
    {
        public DbSet<DummyAggregate> Aggregates => Set<DummyAggregate>();
    }

    private sealed class DummyAggregate : AggregateRoot<Guid>
    {
        public DummyAggregate(Guid id)
        {
            Id = id;
        }
    }

    private sealed class DummyRepository(FakeDbContext dbContext) : RepositoryBase<DummyAggregate>(dbContext);
}
