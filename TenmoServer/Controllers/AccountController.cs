using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao accountDao;

        public AccountController (IAccountDao accountDao)
        {
            this.accountDao = accountDao;
        }


        [HttpGet]
        public ActionResult<Account> GetBalance()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value); 
            Account account = new Account();
            account = accountDao.GetBalance(userId);
            return Ok(account);
            
        }
    }
}
