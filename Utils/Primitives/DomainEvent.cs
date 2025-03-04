using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace Primitives;

[ExcludeFromCodeCoverage]
public abstract record DomainEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
}