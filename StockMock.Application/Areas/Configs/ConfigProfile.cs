using AutoMapper;
using StockMock.Application.Areas.Configs.Dtos;
using StockMock.Domain.Entities.Configs;

namespace StockMock.Application.Areas.Configs
{
    public class ConfigProfile : Profile
    {
        public ConfigProfile()
        {
            CreateMap<Day, DayDto>();
        }
    }
}
