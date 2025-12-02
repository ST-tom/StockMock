using AutoMapper;
using StockMock.Application.Areas.Mocks.Dtos;
using StockMock.Domain.Entities.Mocks;

namespace StockMock.Application.Areas.StockMocks
{
    public class StockMockProfile : Profile
    {
        public StockMockProfile()
        {
            CreateMap<Mock, MockDto>();
            CreateMap<Mock, MockPageDto>();

        }
    }
}
