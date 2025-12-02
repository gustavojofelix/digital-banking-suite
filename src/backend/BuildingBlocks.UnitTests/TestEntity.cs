using BuildingBlocks;

namespace BuildingBlocks.UnitTests;

// Simple test-only entity for equality tests
public sealed class TestEntity : Entity
{
    public string Name { get; }

    public TestEntity(string name)
        : base()
    {
        Name = name;
    }

    public TestEntity(Guid id, string name)
        : base(id)
    {
        Name = name;
    }
}