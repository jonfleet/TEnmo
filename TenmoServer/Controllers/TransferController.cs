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
    [Route("{user}/[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly IUserDAO UserDao;
        private readonly ReturnUser User;

        [HttpGet]
        public ActionResult<List<Transfer>> GetTransfers()
        {
            //Get all Send and Receive Transfers for the user
            try
            {
                List<Transfer> transfers = UserDao.GetTransfers(User);
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
                Transfer singleTransfer = UserDao.GetTransferById(transferId, User);
                return Ok(singleTransfer);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpPost("send/{toUserId}")]
        public ActionResult<Payment> SendPayment(Payment payment)
        {
            // Sends payment to the specified user {toUserId}
            try
            {
                Payment completedPayment = UserDao.SendPayment(payment);
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
