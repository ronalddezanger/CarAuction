
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService;

public class CheckAuctionFinished : BackgroundService
{
    private readonly ILogger<CheckAuctionFinished> _logger;
    private readonly IServiceProvider _services;

    public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting check for Finished Auctions.");

        stoppingToken.Register(() => _logger.LogInformation("Finished Auctions check is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuction(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task CheckAuction(CancellationToken stoppingToken)
    {
        var finishedAuctions = await DB.Find<Auction>()
            .Match(x => x.AuctionEnd <= DateTime.UtcNow)
            .Match(x => !x.Finished)
            .ExecuteAsync(stoppingToken);

        if(finishedAuctions.Count == 0) return;

        _logger.LogInformation("Found {count} finished auctions.", finishedAuctions.Count);

        using var scope = _services.CreateScope();
        var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        foreach (var auction in finishedAuctions)
        {
            // var bid = await DB.Find<Bid>()
            //     .Match(x => x.AuctionId == auction.ID)
            //     .SortDescending(x => x.Amount)
            //     .FirstOrDefaultAsync(stoppingToken);

            // if (bid == null)
            // {
            //     await endpoint.Publish<AuctionFinished>(new
            //     {
            //         AuctionId = auction.ID,
            //         ItemSold = false,
            //         Winner = string.Empty,
            //         Seller = auction.Seller,
            //         Amount = (int?) null
            //     }, stoppingToken);
            // }
            // else
            // {
            //     await endpoint.Publish<AuctionFinished>(new
            //     {
            //         AuctionId = auction.ID,
            //         ItemSold = true,
            //         Winner = bid.Bidder,
            //         Seller = auction.Seller,
            //         Amount = bid.Amount
            //     }, stoppingToken);
            // }

            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);

            var winningBid = await DB.Find<Bid>()
                .Match(x => x.AuctionId == auction.ID)
                .Match(b => b.BidStatus == BidStatus.Accepted)
                .Sort(x => x.Descending(s => s.Amount))
                .ExecuteFirstAsync(stoppingToken);
            await endpoint.Publish(new AuctionFinished
            {
                AuctionId = auction.ID,
                ItemSold = winningBid != null,
                Winner = winningBid?.Bidder,
                Seller = auction.Seller,
                Amount = winningBid?.Amount
            }, stoppingToken);
        }
    }
}
