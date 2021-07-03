using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();

        static void Main(string[] args)
        {
            consoleService.Run();
        }        
    }
}
