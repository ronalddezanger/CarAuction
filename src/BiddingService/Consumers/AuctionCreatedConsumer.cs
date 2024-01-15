using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        var auction = new Auction
        {
            ID = context.Message.Id.ToString(),
            AuctionEnd = context.Message.AuctionEnd,
            ReservePrice = context.Message.ReservePrice,
            Seller = context.Message.Seller
        };

        await auction.SaveAsync();
    }

}
