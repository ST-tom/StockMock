using StockMock.Domain.Entities.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMock.Application.Extensions
{
    public static class StockExstension
    {
        public static decimal GetMaxGain(this BoardType boardType)
        {
            switch (boardType)
            {
                case BoardType.MainBoard:
                    return 10;
                case BoardType.STARMarket:
                    return 20;
                case BoardType.ChiNextBoard:
                    return 20;
                case BoardType.BSE:
                    return 30;
                case BoardType.NEEQInnovationLayer:
                    return 50;
                default:
                    return 0;
            }
        }
    }
}
