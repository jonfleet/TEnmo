using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Exceptions;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly IUserDAO userDao;

        public TransferController(IUserDAO _userDao)
        {
            userDao = _userDao;
        }



        [HttpGet("{userId}/{transferId}")]
        public ActionResult<Transfer> GetTransferById(int userId, int transferId)
        {

            // Get a single transfer based on Id
            try
            {
                Transfer singleTransfer = userDao.GetTransferById(userId, transferId);
                return Ok(singleTransfer);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpPost("send")]
        public ActionResult<Transfer> SendTransfer(Transfer transfer = null)
        {
            // Sends payment to the specified user {toUserId}
            try
            {
                Transfer sentTransfer = userDao.CreateTransfer(transfer, "Send");
                Transfer completedTransfer = userDao.ApproveTransfer(sentTransfer);
                
                return Ok(completedTransfer);
            }
            catch (InsufficientBalanceException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost("request/{fromUserId}")]
        public ActionResult<Transfer> RequestTransfer(Transfer transfer = null)
        {
            try
            {
                Transfer pendingTransfer = userDao.CreateTransfer(transfer, "Request");
                
                return Ok(pendingTransfer);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPut("approve/{transferId}")]
        public ActionResult<Transfer> ApproveTransfer(Transfer transfer = null)
        {
            try
            {
                userDao.CheckUnauthorizedApproval(transfer);
                Transfer approvedTransfer = userDao.ApproveTransfer(transfer);
                return Ok(approvedTransfer);
            }
            catch(StopTryingToApproveYourRequestException)
            {
                return StatusCode(401);
            }
            catch (Exception)
            {                
                return NotFound();                
            }
        }

        [HttpPut("reject/{transferId}")]
        public ActionResult<Transfer> RejectTransfer(Transfer transfer)
        {
            try
            {
                userDao.CheckUnauthorizedApproval(transfer);
                Transfer rejectedTransfer = userDao.RejectTransfer(transfer);
                return Ok(rejectedTransfer);
            }
            catch (StopTryingToApproveYourRequestException)
            {
                return StatusCode(401);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("completed")]
        public void GetCompletedTransfer()
        {
            // Returns all completed Transactions
        }

        
    }
}
