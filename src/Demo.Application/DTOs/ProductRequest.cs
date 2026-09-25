using System.Drawing;

namespace Demo.Application.DTOs;

public sealed record ProductRequest
{
    public string? Name { get; init; }
    public uint Quantity { get; init; }
}
