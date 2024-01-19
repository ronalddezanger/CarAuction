import { Auction, PagedResult } from "@/types";

type State = {
    auctions: Auction[];
    totalCounts: number;
    pageCount: number;
};

type Actions = {
    setData: (data: PagedResult<Auction>) => void;
    setCurrentPrice: (auctionId: string, amount: number) => void;
}