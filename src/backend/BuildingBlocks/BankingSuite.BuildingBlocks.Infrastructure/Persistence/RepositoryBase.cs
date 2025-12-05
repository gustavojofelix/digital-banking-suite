using BankingSuite.BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSuite.BuildingBlocks.Infrastructure.Persistence;

// Generic EF repository for most services (Customer, Account, etc.)
public class RepositoryBase<TAggregate> where TAggregate : AggregateRoot<Guid>
{
    protected readonly DbContext DbContext;
    protected readonly DbSet<TAggregate> DbSet;

    protected RepositoryBase(DbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TAggregate>();
    }

    public Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        => DbSet.AddAsync(aggregate, cancellationToken).AsTask();

    public void Remove(TAggregate aggregate) => DbSet.Remove(aggregate);
}