using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Balance
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public decimal PrimaryBalance { get; set; }

        public Balance(int accountId, int userId, decimal primaryBalance)
        {
            AccountId = accountId;
            UserId = userId;
            PrimaryBalance = primaryBalance;
        }

        public Balance() { }
    }
}
