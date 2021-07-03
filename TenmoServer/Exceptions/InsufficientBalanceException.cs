using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Exceptions
{
    public class InsufficientBalanceException : Exception
    {
        public InsufficientBalanceException () : base("Transfer cancelled. Insufficient Balance.") { }
    }
}
