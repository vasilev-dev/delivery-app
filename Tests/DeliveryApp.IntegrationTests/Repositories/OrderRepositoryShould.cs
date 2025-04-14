using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class OrderRepositoryShould : BaseRepositoryTest
{
    [Fact]
    public async Task NotReturnOrderWhenItDoesNotExist()
    {
        var orderRepository = new OrderRepository(DbContext);

        var actual = await orderRepository.GetById(Guid.NewGuid());

        actual.Should().BeNull();
    }

    [Fact]
    public async Task ReturnOrderWhenItExists()
    {
        var orderRepository = new OrderRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext);

        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        await orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync();
        
        var actual = await orderRepository.GetById(order.Id);
        
        actual.Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task ReturnEmptyCollectionWhenNoAnyCreatedOrders()
    {
        var orderRepository = new OrderRepository(DbContext);
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();
        
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        await orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync();

        courier.SetBusy();
        order.Assign(courier);
        
        courierRepository.Update(courier);
        orderRepository.Update(order);
        
        await unitOfWork.SaveChangesAsync();

        var actual = await orderRepository.GetAllCreated();
        
        actual.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ReturnCreatedOrdersIfItExists()
    {
        var orderRepository = new OrderRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext);
        
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        await orderRepository.Add(order);
        
        await unitOfWork.SaveChangesAsync();
        
        var actual = await orderRepository.GetAllCreated();
        
        actual.Should().HaveCount(1);
        actual.ElementAt(0).Should().BeEquivalentTo(order);
    }
    
    [Fact]
    public async Task ReturnEmptyCollectionWhenNoAnyAssignedOrders()
    {
        var orderRepository = new OrderRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext);
        
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        await orderRepository.Add(order);
        
        await unitOfWork.SaveChangesAsync();
        
        var actual = await orderRepository.GetAllAssigned();
        
        actual.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ReturnAssignedOrdersIfItExists()
    {
        var orderRepository = new OrderRepository(DbContext);
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();
        
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        await orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync();

        courier.SetBusy();
        order.Assign(courier);
        
        courierRepository.Update(courier);
        orderRepository.Update(order);
        
        await unitOfWork.SaveChangesAsync();
        
        var actual = await orderRepository.GetAllAssigned();
        
        actual.Should().HaveCount(1);
        actual.ElementAt(0).Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task Add()
    {
        var orderRepository = new OrderRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext);
        
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        await orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync();
        
        var actual = await orderRepository.GetById(order.Id);
        
        actual.Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task Update()
    {
        var orderRepository = new OrderRepository(DbContext);
        var courierRepository = new CourierRepository(DbContext);
        var unitOfWork = new UnitOfWork(DbContext);
        
        var courier = Courier.Create("Ivan", "Tesla", 3, Location.CreateRandom()).Value;
        await courierRepository.Add(courier);
        await unitOfWork.SaveChangesAsync();
        
        var order = Order.Create(Guid.NewGuid(), Location.CreateRandom()).Value;
        await orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync();

        courier.SetBusy();
        order.Assign(courier);
        
        courierRepository.Update(courier);
        orderRepository.Update(order);
        
        await unitOfWork.SaveChangesAsync();
        
        var actual = await orderRepository.GetById(order.Id);
        
        actual.Should().BeEquivalentTo(order);
    }
}