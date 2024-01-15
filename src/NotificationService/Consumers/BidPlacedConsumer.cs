using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService;

namespace AuctionFinishedConsumer;

public class BidPlacedConsumer : IConsumer<AuctionCreated>
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public BidPlacedConsumer(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"BidPlacedConsumer: {context.Message.Id}");
        await _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}
