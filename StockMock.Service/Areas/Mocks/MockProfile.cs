using AutoMapper;
using StockMock.Core.Mocks;
using StockMock.Service.Areas.Mocks.Dtos;

namespace StockMock.Service.Areas.Mocks
{
    public class MockProfile : Profile
    {
        public MockProfile()
        {
            CreateMap<Mock, MockDto>();
            CreateMap<Mock, MockPageDto>();
            CreateMap<MockDate, MockDateDto>();
            CreateMap<MockDate, MockDatePageDto>();

            CreateMap<Mock, MockInfoDetailDto>();
            CreateMap<MockDate, MockInfoDateDto>();
        }
    }
}
