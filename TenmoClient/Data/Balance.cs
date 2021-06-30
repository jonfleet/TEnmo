using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Balance
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public decimal PrimaryBalance { get; set; }
    }
}
