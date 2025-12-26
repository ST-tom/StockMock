using AutoMapper;
using StockMock.Core.Configs;
using StockMock.Service.Areas.Configs.Dtos;

namespace StockMock.Service.Areas.Configs
{
    public class ConfigProfile : Profile
    {
        public ConfigProfile()
        {
            CreateMap<Day, DayDto>();
        }
    }
}
