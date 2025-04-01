using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;
using Errors = DeliveryApp.Core.Application.UseCases.Commands.CreateOrder.Errors;

namespace DeliveryApp.UnitTests.Application.UseCases.Commands;

public class CreateOrderHandlerShould
{
    private readonly IOrderRepository _orderRepositoryMock = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    private readonly IGeoClient _geoClientMock = Substitute.For<IGeoClient>();

    [Fact]
    public async Task ReturnErrorIfOrderAlreadyExists()
    {
        var existedOrder = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        
        _orderRepositoryMock.GetById(Arg.Any<Guid>())
            .Returns(Task.FromResult(existedOrder));

        var getGeolocationResult = Result.Success<Location, Error>(Location.CreateRandom());
        _geoClientMock.GetGeolocation(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(getGeolocationResult));

        var command = new CreateOrderCommand(existedOrder.Id, "ул. Пушкина");
        var handler = new CreateOrderHandler(_orderRepositoryMock, _unitOfWorkMock, _geoClientMock);
        
        var actual = await handler.Handle(command, CancellationToken.None);
        
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(Errors.OrderAlreadyExists(existedOrder.Id));
    }

    [Fact]
    public async Task ReturnErrorIfCanNotGetLocation()
    {
        _orderRepositoryMock.GetById(Arg.Any<Guid>())
            .Returns(Task.FromResult<Order>(null));
        
        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        
        var getGeolocationResult = Result.Failure<Location, Error>(new Error("code", "message"));
        _geoClientMock.GetGeolocation(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(getGeolocationResult));
        
        var command = new CreateOrderCommand(Guid.NewGuid(), "ул. Пушкина");
        var handler = new CreateOrderHandler(_orderRepositoryMock, _unitOfWorkMock, _geoClientMock);
        
        var actual = await handler.Handle(command, CancellationToken.None);
        
        actual.IsSuccess.Should().BeFalse();
        actual.Error.Should().Be(getGeolocationResult.Error);
    }

    [Fact]
    public async Task CreateOrder()
    {
        _orderRepositoryMock.GetById(Arg.Any<Guid>())
            .Returns(Task.FromResult<Order>(null));
        
        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));
        
        var getGeolocationResult = Task.FromResult(Result.Success<Location, Error>(Location.CreateRandom()));
        _geoClientMock.GetGeolocation(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(getGeolocationResult);
        
        var command = new CreateOrderCommand(Guid.NewGuid(), "ул. Пушкина");
        var handler = new CreateOrderHandler(_orderRepositoryMock, _unitOfWorkMock, _geoClientMock);
        
        var actual = await handler.Handle(command, CancellationToken.None);
        
        actual.IsSuccess.Should().BeTrue();
        
        await _orderRepositoryMock.Received(1)
            .Add(Arg.Is<Order>(o => o.Id == command.BasketId));
        
        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}