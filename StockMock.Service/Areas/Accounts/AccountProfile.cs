using AutoMapper;
using StockMock.Core.Accounts;
using StockMock.Service.Areas.Accounts.Dtos;
using TS.Shared.Util;

namespace StockMock.Service.Areas.Accounts
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            // Account -> AccountInfoDto
            CreateMap<Account, AccountInfoDto>()
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role.GetDescription()));

            // AccountDto -> Account（新增）
            CreateMap<AccountDto, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => EncryptionUtil.ToMD5(src.Password)));

            // AccountDto -> Account（修改）
            CreateMap<AccountDto, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LoginAccount, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginTime, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorName, opt => opt.Ignore())
                .ForMember(dest => dest.CreateTime, opt => opt.Ignore());
        }
    }
}
