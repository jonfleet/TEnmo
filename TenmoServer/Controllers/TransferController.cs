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
        private readonly ReturnUser user;

        public TransferController(IUserDAO _userDao, ReturnUser _user)
        {
            userDao = _userDao;
            user = _user;
        }

        [HttpGet]
        public ActionResult<List<Transfer>> GetTransfers()
        {
            
            try
            {
                List<Transfer> transfers = userDao.GetTransfers(user);
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
            // Get a single transfer based on Id
            try
            {
                Transfer singleTransfer = userDao.GetTransferById(transferId, user);
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
