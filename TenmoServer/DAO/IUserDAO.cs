using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDAO
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();
        Balance GetBalance(string username);
       
        List<Transfer> GetTransfers(string username);
        Transfer GetTransferById(int id, string username);

        Transfer SendPayment(Transfer payment);
    }
}
