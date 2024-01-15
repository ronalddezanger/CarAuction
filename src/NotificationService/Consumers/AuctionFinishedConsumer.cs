using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService;

public class AuctionFinishedConsumer : IConsumer<AuctionCreated>
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public AuctionFinishedConsumer(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"AuctionFinishedConsumer: {context.Message.Id}");
        await _hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
    }
}
