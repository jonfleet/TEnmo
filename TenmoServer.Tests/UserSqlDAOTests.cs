using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;
using TenmoServer.DAO;
using TenmoServer.Models;

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
        private readonly Transfer TransferRequestPending = new Transfer(1, 1, 1, 1, "test1", 2, "test2", 100.0M);
        private readonly Transfer TransferRequestApproved = new Transfer(1, 1, 2, 1,"test1", 2, "test2", 100.0M);
        private readonly Transfer TransferRequestRejected = new Transfer(1, 1, 3, 1, "test1", 2, "test2", 100.0M);
        private readonly Transfer TransferSendPending = new Transfer(1, 2, 1, 1, "test1", 2, "test2", 100.0M);
        private readonly Transfer TransferSendApproved = new Transfer(1, 2, 2, 1, "test1", 2, "test2", 100.0M);
        private readonly Transfer TransferSendRejected = new Transfer(1, 2, 3, 1, "test1", 2, "test2", 100.0M);

        private readonly Transfer Transfer1 = new Transfer(1, 1, 1, 1, "test1", 2, "test2", 100.0M);
        private readonly Transfer Transfer2 = new Transfer(2, 1, 2, 1, "test1", 2, "test2", 100.0M);
        private readonly Transfer Transfer3 = new Transfer(3, 1, 3, 2, "test2", 3, "test3", 300.0M);
        private readonly Transfer Transfer4 = new Transfer(4, 2, 2, 3, "test3", 1, "test1", 400.0M);
        private readonly Transfer Transfer5 = new Transfer(5, 1, 1, 1, "test1", 3, "test3", 500.0M);

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
        public void GetTransfers_ReturnAllUser3Transfers()
        {
            

            UserSqlDAO dao = new UserSqlDAO(ConnectionString);
            List<Transfer> expectedTransfers = new List<Transfer>();
            expectedTransfers.Add(Transfer3);
            expectedTransfers.Add(Transfer4);
            expectedTransfers.Add(Transfer5);

            List<Transfer> actualTransfers = dao.GetTransfers(3);
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

            Transfer actualTransfer2 = dao.GetTransferById(2, 2);
            Transfer actualTransfer3 = dao.GetTransferById(2, 3);
            Transfer actualTransfer4 = dao.GetTransferById(3, 4);
            
            AssertTransferMatch(Transfer2, actualTransfer2);
            AssertTransferMatch(Transfer3, actualTransfer3);
            AssertTransferMatch(Transfer4, actualTransfer4);
            
        }

        [TestMethod]
        public void CreateTransfer()
        {
            IUserDAO dao = new UserSqlDAO(ConnectionString);
            Transfer newTransfer = new Transfer();
            newTransfer.FromUserId = 1;
            newTransfer.ToUserId = 2;
            newTransfer.Amount = 100;

            // Test Request Transfers
            Transfer actualRequestTransfer = dao.CreateTransfer(newTransfer, "Request");

            // Compare excluding transferId
            Assert.AreEqual(TransferRequestPending.TransferStatusId, actualRequestTransfer.TransferStatusId);
            Assert.AreEqual(TransferRequestPending.TransferTypeId, actualRequestTransfer.TransferTypeId);
            Assert.AreEqual(TransferRequestPending.ToUserId, actualRequestTransfer.ToUserId);
            Assert.AreEqual(TransferRequestPending.FromUserId, actualRequestTransfer.FromUserId);
            Assert.AreEqual(TransferRequestPending.Amount, actualRequestTransfer.Amount);

            // Test Send Transfer
            Transfer actualSendTransfer = dao.CreateTransfer(newTransfer, "Send");

            // Compare excluding transferId
            Assert.AreEqual(TransferSendPending.TransferStatusId, actualSendTransfer.TransferStatusId);
            Assert.AreEqual(TransferSendPending.TransferTypeId, actualSendTransfer.TransferTypeId);
            Assert.AreEqual(TransferRequestPending.ToUserId, actualSendTransfer.ToUserId);
            Assert.AreEqual(TransferRequestPending.FromUserId, actualSendTransfer.FromUserId);
            Assert.AreEqual(TransferRequestPending.Amount, actualSendTransfer.Amount);

        }

        [TestMethod]
        public void ApproveRequestTransfer()
        {
            IUserDAO dao = new UserSqlDAO(ConnectionString);

            // Test Approval of TransferRequestPending
            Transfer actualRequestApproved = dao.ApproveTransfer(TransferRequestPending);

            Assert.AreEqual(TransferRequestApproved.TransferStatusId, actualRequestApproved.TransferStatusId);
            Assert.AreEqual(TransferRequestApproved.TransferTypeId, actualRequestApproved.TransferTypeId);
            Assert.AreEqual(TransferRequestApproved.ToUserId, actualRequestApproved.ToUserId);
            Assert.AreEqual(TransferRequestApproved.FromUserId, actualRequestApproved.FromUserId);
            Assert.AreEqual(TransferRequestApproved.Amount, actualRequestApproved.Amount);


            // Check to make sure proper amount is withdrawn and deposited
            Balance actualUser1Balance = dao.GetBalance(1);
            decimal expectedUser1Balance = Balance1.PrimaryBalance - TransferRequestPending.Amount;

            Assert.AreEqual(expectedUser1Balance, actualUser1Balance.PrimaryBalance);

            Balance actualUser2Balance = dao.GetBalance(2);
            decimal expectedUser2Balance = Balance2.PrimaryBalance + TransferRequestPending.Amount;

            Assert.AreEqual(expectedUser2Balance, actualUser2Balance.PrimaryBalance);

        }

        [TestMethod]
        public void ApproveSendTransfer()
        {
            IUserDAO dao = new UserSqlDAO(ConnectionString);

            // Test Approval of TransferRequestPending
            Transfer actualSendApproved = dao.ApproveTransfer(TransferSendPending);

            Assert.AreEqual(TransferSendApproved.TransferStatusId, actualSendApproved.TransferStatusId);
            Assert.AreEqual(TransferSendApproved.TransferTypeId, actualSendApproved.TransferTypeId);
            Assert.AreEqual(TransferSendApproved.ToUserId, actualSendApproved.ToUserId);
            Assert.AreEqual(TransferSendApproved.FromUserId, actualSendApproved.FromUserId);
            Assert.AreEqual(TransferSendApproved.Amount, actualSendApproved.Amount);


            // Check to make sure proper amount is withdrawn and deposited
            Balance actualUser1Balance = dao.GetBalance(1);
            decimal expectedUser1Balance = Balance1.PrimaryBalance - TransferSendPending.Amount;

            Assert.AreEqual(expectedUser1Balance, actualUser1Balance.PrimaryBalance);

            Balance actualUser2Balance = dao.GetBalance(2);
            decimal expectedUser2Balance = Balance2.PrimaryBalance + TransferSendPending.Amount;

            Assert.AreEqual(expectedUser2Balance, actualUser2Balance.PrimaryBalance);
        }

        [TestMethod]
        public void RejectRequestTransfer()
        {
            IUserDAO dao = new UserSqlDAO(ConnectionString);

            // Test Approval of TransferRequestPending
            Transfer actualRequestRejected = dao.RejectTransfer(TransferRequestPending);

            Assert.AreEqual(TransferRequestRejected.TransferStatusId, actualRequestRejected.TransferStatusId);
            Assert.AreEqual(TransferRequestRejected.TransferTypeId, actualRequestRejected.TransferTypeId);
            Assert.AreEqual(TransferRequestRejected.ToUserId, actualRequestRejected.ToUserId);
            Assert.AreEqual(TransferRequestRejected.FromUserId, actualRequestRejected.FromUserId);
            Assert.AreEqual(TransferRequestRejected.Amount, actualRequestRejected.Amount);
        }
        [TestMethod]
        public void RejectSendTransfer()
        {
            IUserDAO dao = new UserSqlDAO(ConnectionString);

            // Test Approval of TransferRequestPending
            Transfer actualSendRejected = dao.RejectTransfer(TransferSendPending);

            Assert.AreEqual(TransferSendRejected.TransferStatusId, actualSendRejected.TransferStatusId);
            Assert.AreEqual(TransferSendRejected.TransferTypeId, actualSendRejected.TransferTypeId);
            Assert.AreEqual(TransferSendRejected.ToUserId, actualSendRejected.ToUserId);
            Assert.AreEqual(TransferSendRejected.FromUserId, actualSendRejected.FromUserId);
            Assert.AreEqual(TransferSendRejected.Amount, actualSendRejected.Amount);
        }
       
        [TestMethod]
        public void GetPendingTransfersForUser()
        {
            IUserDAO dao = new UserSqlDAO(ConnectionString);

            List<Transfer> expectedPendingTransfers = new List<Transfer>();
            expectedPendingTransfers.Add(Transfer1);
            expectedPendingTransfers.Add(Transfer5);

            List<Transfer> actualPendingTransfers = dao.GetPendingTransfersForUser(1);
            if(actualPendingTransfers.Count != expectedPendingTransfers.Count)
            {
                Assert.Fail("The query did not return the expected number of transfers");
            }
            for(int i = 0; i < expectedPendingTransfers.Count; i++)
            {
                AssertTransferMatch(expectedPendingTransfers[i], actualPendingTransfers[i]);
            }
            
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
