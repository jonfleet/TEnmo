using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public string ToUserId { get; set; }
        public string ToUsername { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
    }
}
