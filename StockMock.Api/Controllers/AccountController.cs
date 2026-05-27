using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMock.Core;
using StockMock.Service.Areas.Accounts.Dtos;
using StockMock.Service.Areas.Accounts.Services;

namespace StockMock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController(IUser user, AccountService accountService) : ControllerBase
    {
        private readonly IUser _user = user;
        private readonly AccountService _accountService = accountService;

    }
}
