using AutoMapper;
using StockMock.Core.Stocks;
using StockMock.Service.Areas.Stocks.Dtos;

namespace StockMock.Service.Areas.Stocks
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<AccountStock, AccountStockDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StockName))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.StockCode));
            CreateMap<Stock, AccountStockDto>();
            CreateMap<StockDate, StockDateDto>();
        }
    }
}
