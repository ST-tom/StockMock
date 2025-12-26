using AutoMapper;
using StockMock.Core.Mocks;
using StockMock.Service.Areas.Mocks.Dtos;

namespace StockMock.Service.Areas.Mocks
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
