using System;

namespace Demo.Domain;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public uint Quantity { get; private set; }

    private Product() { }


    public void Update(string? name, uint quantity)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidException("Name is required");
        }

        Name = name;
        Quantity = quantity;
    }

    public static Product Create(string? name, uint quantity)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidException("Name is required");
        }

        return new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Quantity = quantity
        };
    }
}
