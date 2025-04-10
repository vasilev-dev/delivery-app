using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using MediatR;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class CourierRepositoryShould : BaseRepositoryTest
{
    [Fact]
    public async Task NotReturnCourierWhenItDoesNotExist()
    {
        var courierRepository = new CourierRepository(DbContext);
        
        var actual = await courierRepository.GetById(Guid.NewGuid());
        
        actual.Should().BeNull();
    }
    
    [Fact]
    public async Task ReturnCourierWhenItExists()
    {
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext, Mediator);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();

        var actual = await courierRepository.GetById(courier.Id);
        
        actual.Should().BeEquivalentTo(courier);
    }

    [Fact]
    public async Task ReturnEmptyCollectionWhenNoAnyFreeCouriers()
    {
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext, Mediator);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        courier.SetBusy();
        
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();

        var actual = await courierRepository.GetAllFree();
        
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task ReturnFreeCouriersWhenItExists()
    {
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext, Mediator);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();

        var actual = await courierRepository.GetAllFree();

        actual.Should().HaveCount(1);
        actual.ElementAt(0).Should().BeEquivalentTo(courier);
    }

    [Fact]
    public async Task AddCourier()
    {
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext, Mediator);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();

        var actual = await courierRepository.GetById(courier.Id);
        
        actual.Should().BeEquivalentTo(courier);
    }

    [Fact]
    public async Task UpdateCourier()
    {
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext, Mediator);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();

        var actual = await courierRepository.GetById(courier.Id);
        
        actual.Should().BeEquivalentTo(courier);
    }
}