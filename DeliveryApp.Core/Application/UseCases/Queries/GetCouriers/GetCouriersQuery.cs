using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCouriers;

public record GetCouriersQuery : IRequest<GetCouriersResult>;