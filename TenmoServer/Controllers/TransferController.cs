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
    //[Authorize]
    public class TransferController : ControllerBase
    {
        private readonly IUserDAO userDao;

        public TransferController(IUserDAO _userDao)
        {
            userDao = _userDao;
        }

        [HttpGet]
        public ActionResult<List<Transfer>> GetTransfers()
        {
            string username = User.Identity.Name;
            try
            {
                List<Transfer> transfers = userDao.GetTransfers(username);
                return Ok(transfers);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("{transferId}")]
        public ActionResult<Transfer> GetTransferById(int transferId)
        {
            string username = User.Identity.Name;

            // Get a single transfer based on Id
            try
            {
                Transfer singleTransfer = userDao.GetTransferById(transferId, username);
                return Ok(singleTransfer);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpPost("send/{toUserId}")]
        public ActionResult<Transfer> SendPayment(Transfer payment)
        {
            // Sends payment to the specified user {toUserId}
            try
            {
                Transfer completedPayment = userDao.SendPayment(payment);
                return Ok(completedPayment);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost("request/{fromUserId}")]
        public void RequestPayment()
        {
            // Requests Payment from specified user {fromUserId}
        }
        [HttpGet("pending")]
        public void GetPendingTransactions()
        {
            // Returns all pending transactions
        }

        [HttpGet("completed")]
        public void GetCompletedTransactions()
        {
            // Returns all completed Transactions
        }

        [HttpPost("approve/{transferId}")]
        public void ApproveTransfer()
        {
            // Approve Transfer by {transferId}
        }

        [HttpPost("reject/{transferId}")]
        public void RejectTransfer()
        {
            // reject transfer by {transferId}
        }
    }
}
