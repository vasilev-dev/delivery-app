using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignCourier;

public record AssignCourierCommand : IRequest<UnitResult<Error>>;