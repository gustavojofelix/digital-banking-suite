namespace BankingSuite.BuildingBlocks.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}