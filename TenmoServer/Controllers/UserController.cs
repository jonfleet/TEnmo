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
       
        
        public UserController(IUserDAO _userDAO)
        {
            userDAO = _userDAO;
        }
        
        [HttpGet("balance")]
        public ActionResult<Balance> GetTheBalance()
        {
            string username = User.Identity.Name;

            try
            {
                Balance balance = userDAO.GetBalance(username.Length);
                if(balance.UserId != 0)
                {
                return Ok(balance);
                }
                return BadRequest("The database did not communicate correctly");
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
