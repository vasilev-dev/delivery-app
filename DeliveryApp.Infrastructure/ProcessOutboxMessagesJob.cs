using System.Diagnostics.CodeAnalysis;
using Ardalis.SmartEnum.JsonNet;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Primitives;
using Quartz;
using DomainOrderStatus = DeliveryApp.Core.Domain.Model.OrderAggregate.OrderStatus;

namespace DeliveryApp.Infrastructure;

[ExcludeFromCodeCoverage]
[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob(AppDbContext dbContext, IPublisher publisher) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var outboxMessages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(o => o.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        if (outboxMessages.Count == 0)
        {
            return;
        }
        
        foreach (var outboxMessage in outboxMessages)
        {
            var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(outboxMessage.Content,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Converters = new List<JsonConverter>
                    {
                        new SmartEnumNameConverter<DomainOrderStatus, int>()
                    }
                });

            
            await publisher.Publish(domainEvent, context.CancellationToken);
            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }
        
        await dbContext.SaveChangesAsync();
    }
}
