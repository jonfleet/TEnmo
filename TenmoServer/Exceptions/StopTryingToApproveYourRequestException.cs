using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Exceptions
{
    public class StopTryingToApproveYourRequestException : Exception
    {
        public StopTryingToApproveYourRequestException() : base("Not funny") { }
    }
}
