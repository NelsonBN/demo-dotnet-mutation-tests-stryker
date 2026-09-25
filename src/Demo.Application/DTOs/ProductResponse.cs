using Demo.Domain;
using System;

namespace Demo.Application.DTOs;

public sealed record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public uint Quantity { get; init; }

    public static implicit operator ProductResponse(Product product)
        => new() 
        { 
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity
        };
}