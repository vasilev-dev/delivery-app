using DeliveryApp.Core.Application.UseCases.Commands.AssignCourier;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class AssignCourierJob(IMediator mediator) : IJob
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Execute(IJobExecutionContext context)
    {
        var assignOrdersCommand = new AssignCourierCommand();
        
        await _mediator.Send(assignOrdersCommand);
    }
}