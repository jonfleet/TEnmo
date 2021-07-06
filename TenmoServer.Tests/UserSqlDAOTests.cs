using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System.IO;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TenmoServer;
using TenmoServer.DAO;
using TenmoServer.Models;
using System.Collections.Generic;

namespace TenmoServer.Tests
{
    [TestClass]
    public class UserSqlDAOTests
    {
        public static IConfiguration Configuration { get; }
        private string ConnectionString = "Server=.\\SQLEXPRESS;Database=tenmo;Trusted_Connection=True;";
        private TransactionScope transaction;

        // Sample Users
        private readonly User User1 = new User(1, "test1", "password1", "123");
        private readonly User User2 = new User(2, "test2", "password2", "123");
        private readonly User User3 = new User(3, "test3", "password3", "123");
        private readonly User ExpectedNewUser = new User(4, "test4", "", "");

        // Sample Transfers
        private readonly Transfer Transfer1 = new Transfer(1, 2, 2, 1, "test1", 2, "test2", 100.0M);
        private readonly Transfer Transfer2 = new Transfer(2, 2, 2, 1,"test1", 3, "test3", 200.0M);
        private readonly Transfer Transfer3 = new Transfer(3, 2, 2, 2, "test2", 3, "test3", 300.0M);

        //Sample Balances
        //(1,1, 1000),
        //(2,2, 1000),
        //(3, 3, 1000);
        private readonly Balance Balance1 = new Balance(1, 1, 1000);
        private readonly Balance Balance2 = new Balance(2, 2, 1500);
        
        
        [TestInitialize]
        public void Setup()
        {
            // Begin the transaction
            transaction = new TransactionScope();

            // Get the SQL Script to run
            string testData = File.ReadAllText("test-data.sql");
            //string ConnectionString = Configuration.GetConnectionString("TestProject");
            
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(testData, conn);
                cmd.ExecuteScalar();
            }
        }
            
        [TestMethod]
        public void GetUsersTest_ReturnAllUsers()
        {
            UserSqlDAO dao = new UserSqlDAO(ConnectionString);
            List<User> expectedUsers = new List<User>();
            
            expectedUsers.Add(User1);
            expectedUsers.Add(User2);
            expectedUsers.Add(User3);
            
            List<User> actualUsers = dao.GetUsers();
            if(expectedUsers.Count != actualUsers.Count)
            {
                Assert.Fail("The Query did not return the expected number of users");
            }
            for (int i = 0; i < expectedUsers.Count; i++)
            {
                AssertUsersMatch(actualUsers[i], expectedUsers[i]);
            }
        }
        [TestMethod]
        public void GetUserByUsername_ReturnUser1()
        {
            UserSqlDAO dao = new UserSqlDAO(ConnectionString);

            User actualUser = dao.GetUser("test1");

            AssertUsersMatch(actualUser, User1);
        }

        private void AssertUsersMatch(User actual, User expected)
        {
            Assert.AreEqual(actual.UserId, expected.UserId);
            Assert.AreEqual(actual.Username, expected.Username);
            Assert.AreEqual(actual.PasswordHash, expected.PasswordHash);
            Assert.AreEqual(actual.Salt, expected.Salt);
        }

        [TestMethod]
        public void AddUser_ReturnNewUser()
        {
            UserSqlDAO dao = new UserSqlDAO(ConnectionString);
            User actualNewUser = dao.AddUser("test4", "test4");

            Assert.AreEqual(ExpectedNewUser.Username, actualNewUser.Username);
        }

        [TestMethod]
        public void GetTransfers_ReturnAllTest2Transfers()
        {
            

            UserSqlDAO dao = new UserSqlDAO(ConnectionString);
            List<Transfer> expectedTransfers = new List<Transfer>();
            expectedTransfers.Add(Transfer1);
            expectedTransfers.Add(Transfer3);

            List<Transfer> actualTransfers = dao.GetTransfers(2);
            if(actualTransfers.Count != expectedTransfers.Count)
            {
                Assert.Fail("The query did not return the expected number of transfers");
            }
            for(int i = 0; i < actualTransfers.Count; i++)
            {
                AssertTransferMatch(actualTransfers[i], expectedTransfers[i]);
            }

        }
        [TestMethod]
        public void GetTransfersById_ReturnTransfer2()
        {
            UserSqlDAO dao = new UserSqlDAO(ConnectionString);

            Transfer actualTransfer1 = dao.GetTransferById(2, 1);
            Transfer actualTransfer2 = dao.GetTransferById(1, 2);
            Transfer actualTransfer3 = dao.GetTransferById(3, 3);
            
            AssertTransferMatch(Transfer1, actualTransfer1);
            AssertTransferMatch(Transfer2, actualTransfer2);
            AssertTransferMatch(Transfer3, actualTransfer3);
            
        }

        [TestMethod]
        public void CreateTransfer()
        {
            UserSqlDAO dao = new UserSqlDAO(ConnectionString);
            Transfer newTransfer = new Transfer();
            newTransfer.FromUserId = 1;
            newTransfer.ToUserId = 2;
            newTransfer.Amount = 100;

            Transfer actualTransfer = dao.CreateTransfer(newTransfer, "Send");

            // Compare to Sample Transfer 1 excluding transferId
            Assert.AreEqual(Transfer1.TransferStatusId, actualTransfer.TransferStatusId);
            Assert.AreEqual(Transfer1.TransferTypeId, actualTransfer.TransferTypeId);
            Assert.AreEqual(Transfer1.ToUserId, actualTransfer.ToUserId);
            Assert.AreEqual(Transfer1.FromUserId, actualTransfer.FromUserId);
            Assert.AreEqual(Transfer1.Amount, actualTransfer.Amount);

            // Check to make sure proper amount is withdrawn and deposited
            Balance actualUser1Balance = dao.GetBalance(1);
            decimal expectedUser1Balance = Balance1.PrimaryBalance - newTransfer.Amount;

            Assert.AreEqual(expectedUser1Balance, actualUser1Balance.PrimaryBalance);

            Balance actualUser2Balance = dao.GetBalance(2);
            decimal expectedUser2Balance = Balance2.PrimaryBalance + newTransfer.Amount;

            Assert.AreEqual(expectedUser2Balance, actualUser2Balance.PrimaryBalance);

        }
       
        private void AssertTransferMatch(Transfer expected, Transfer actual)
        {
            Assert.AreEqual(expected.TransferId, actual.TransferId);
            Assert.AreEqual(expected.TransferTypeId, actual.TransferTypeId);
            Assert.AreEqual(expected.TransferStatusId, actual.TransferStatusId);
            Assert.AreEqual(expected.FromUserId, actual.FromUserId);
            Assert.AreEqual(expected.ToUserId, actual.ToUserId);
            Assert.AreEqual(expected.Amount, actual.Amount);
        }
        [TestMethod]
        public void GetBalance_ReturnUserBalance()
        {
            UserSqlDAO dao = new UserSqlDAO(ConnectionString);

            Balance actualUser1Balance = dao.GetBalance(1);
            Balance actualUser2Balance = dao.GetBalance(2);

            AssertBalanceMatch(Balance1, actualUser1Balance);
            AssertBalanceMatch(Balance2, actualUser2Balance);
        }

        private void AssertBalanceMatch(Balance expected, Balance actual)
        {
            Assert.AreEqual(expected.AccountId, actual.AccountId);
            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.PrimaryBalance, actual.PrimaryBalance);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Roll back the transaction
            transaction.Dispose();
        }
    }
}
