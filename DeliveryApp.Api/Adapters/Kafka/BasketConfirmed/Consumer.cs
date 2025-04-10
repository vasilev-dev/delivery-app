using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BasketConfirmed;
using Confluent.Kafka;
using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using Error = Primitives.Error;

namespace DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;

[ExcludeFromCodeCoverage]
public class Consumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Consumer> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string _topic;

    public Consumer(IServiceScopeFactory serviceScopeFactory, IOptions<Settings> settings, ILogger<Consumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _topic = settings.Value.BasketConfirmedTopic;

        var config = new ConsumerConfig
        {
            BootstrapServers = settings.Value.MessageBrokerHost,
            GroupId = "DeliveryConsumerGroup",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true
        };
        
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    
                    var consumeResult = _consumer.Consume(cancellationToken);
                    if (consumeResult.IsPartitionEOF) continue;
                    _logger.LogInformation("Received message: {message}", consumeResult.Message.Value);

                    var commandResult = await ProcessMessage(consumeResult.Message.Value);

                    if (commandResult.IsFailure)
                    {
                        _logger.LogError("Command error occurred: {code}: {reason}", commandResult.Error.Code, commandResult.Error.Message);
                    }
                    
                    _consumer.Commit(consumeResult);
                    _consumer.StoreOffset(consumeResult);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError("Consume error occurred: {reason}", e.Error.Reason);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error occurred: {reason}", e.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("Operation has been cancelled");
        }
    }
    
    private async Task<UnitResult<Error>> ProcessMessage(string message)
    {
        var @event = JsonSerializer.Deserialize<BasketConfirmedIntegrationEvent>(message);
        var createOrderCommand = new CreateOrderCommand(Guid.Parse(@event.BasketId), @event.Address.Street);
        
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>();
        
        return await mediator.Send(createOrderCommand);
    }
}