using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        private string _transferTypeDesc { get; set; }
        public string TransferTypeDesc { 
            get
            {
                if(TransferTypeId == 1)
                {
                    return "Request";
                }
                else if (TransferTypeId == 2)
                {
                    return "Send";
                }
                else
                {
                    return "Unknown Type";
                }
            }
            set
            {
                _transferTypeDesc = value;
            }
        }
        public int TransferStatusId { get; set; }
        private string _transferStatusDesc { get; set; }
        public string TransferStatusDesc {
            get
            {
                if (TransferStatusId == 1)
                {
                    return ("Pending");
                }
                else if (TransferStatusId == 2)
                {
                    return ("Approved");
                }
                else if (TransferStatusId == 3)
                {
                    return "Rejected";
                }
                else
                {
                    return "Unknown Status";
                }
            }
            set
            {
                _transferStatusDesc = value;
            }
        }
        public int FromUserId { get; set; }
        public string FromUsername { get; set; }
        public int ToUserId { get; set; }
        public string ToUsername { get; set; }
        public decimal Amount { get; set; }
    }
}
