using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetUncompletedOrders;

public record GetUncompletedOrdersQuery : IRequest<GetUncompletedOrdersResult>;