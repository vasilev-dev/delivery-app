using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public record GetBusyCouriersQuery : IRequest<GetBusyCouriersResult>;