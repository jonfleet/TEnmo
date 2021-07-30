using Microsoft.AspNetCore.Authorization;
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
        
        [HttpGet("{userId}/balance")]
        public ActionResult<Balance> GetTheBalance(int userId)
        {

            try
            {
                Balance balance = userDAO.GetBalance(userId);
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
        [HttpGet("{userId}/transfer")]
        public ActionResult<List<Transfer>> GetTransfers(int userId)
        {
            
            try
            {
                List<Transfer> transfers = userDAO.GetTransfers(userId);
                return Ok(transfers);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpGet("{userId}/pending")]
        public ActionResult<Transfer> GetPendingTransfers(int userId)
        {
            try
            {             
                List<Transfer> transfers = userDAO.GetPendingTransfersForUser(userId);
                return Ok(transfers);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
