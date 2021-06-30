using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly IUserDAO userDao;

        [HttpGet()]
        public void GetTransfers()
        {
            //Get all Send and Receive Transfers for the user
            
            //userDao.GetTransfer(user)
        }

        [HttpGet("{transferId}")]
        public void GetTransferById()
        {
            // Get a single transfer based on Id
        }
        [HttpPost("send/{toUserId}")]
        public void SendPayment()
        {
            // Sends payment to the specified user {toUserId} 
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
