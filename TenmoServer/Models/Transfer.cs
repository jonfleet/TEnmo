using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }

        public Transfer(int transferId, int transferTypeId, int tansferStatusId, int fromUserId, int toUserId, decimal amount)
        {
            TransferId = transferId;
            TransferTypeId = transferTypeId;
            TransferStatusId = tansferStatusId;
            FromUserId = fromUserId;
            ToUserId = toUserId;
            Amount = amount;
        }

        public Transfer() { }
    }
}
