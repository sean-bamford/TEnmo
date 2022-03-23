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
    public class UserController : ControllerBase
    {
        private readonly IUserDao userDao;

        public UserController(IUserDao userDao)
        {
            this.userDao = userDao;
        }


        [HttpGet]
        public ActionResult<List<User>> GetUsers()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            List<User> users = userDao.GetUsers();
            foreach (User user in users.ToList())
            {
                if (userId == user.UserId)
                {
                    users.Remove(user);
                }
            }
            return Ok(users);
        }
    }
}
