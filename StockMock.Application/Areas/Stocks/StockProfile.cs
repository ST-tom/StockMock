using AutoMapper;
using StockMock.Application.Areas.Stocks.Dtos;
using StockMock.Domain.Entities.Stocks;

namespace StockMock.Application.Areas.Stocks
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
