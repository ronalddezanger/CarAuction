using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService;

namespace NotificationService;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public BidPlacedConsumer(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine($"BidPlacedConsumer: {context.Message.Id}");
        await _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}
