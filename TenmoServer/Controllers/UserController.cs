using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDAO userDAO;
        private readonly ReturnUser user;
        
        public UserController(IUserDAO _userDAO, ReturnUser _user)
        {
            userDAO = _userDAO;
            user = _user;
        }
        
        [HttpGet("balance")]
        public ActionResult<Balance> GetTheBalance() 
        {
            try
            {
                Balance balance = userDAO.GetBalance(user);
                return Ok(balance);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                List<User> users = userDAO.GetUsers();
                return Ok(users);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
