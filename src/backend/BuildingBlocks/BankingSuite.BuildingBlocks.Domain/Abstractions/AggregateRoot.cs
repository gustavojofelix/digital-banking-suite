using BankingSuite.BuildingBlocks.Domain.Abstractions;
using BankingSuite.BuildingBlocks.Domain.Events;

namespace BankingSuite.BuildingBlocks.Domain.Entities;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    public void ClearDomainEvents() => _domainEvents.Clear();
}