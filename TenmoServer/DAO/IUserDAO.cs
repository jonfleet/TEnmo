using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDAO
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();
        Balance GetBalance(int userId);
       
        List<Transfer> GetTransfers(int userId);
        Transfer GetTransferById(int userId, int transferId);

        Transfer CreateTransfer(Transfer payment, string transferType);
    }
}
