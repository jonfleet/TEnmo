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
        public void GetUsersTest()
        {
            UserSqlDAO dao = new UserSqlDAO(ConnectionString);
            List<User> expectedUsers = new List<User>();
            User testUser1 = new User();
            testUser1.UserId = 1;
            testUser1.Username = "test1";
            testUser1.PasswordHash = "password1";
            testUser1.Salt = "123";
            expectedUsers.Add(testUser1);
            
            User testUser2 = new User();
            testUser2.UserId = 2;
            testUser2.Username = "test2";
            testUser2.PasswordHash = "password2";
            testUser2.Salt = "123";
            expectedUsers.Add(testUser2);

            User testUser3 = new User();
            testUser3.UserId = 3;
            testUser3.Username = "test3";
            testUser3.PasswordHash = "password3";
            testUser3.Salt = "123";
            expectedUsers.Add(testUser3);
            
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

        private void AssertUsersMatch(User actual, User expected)
        {
            Assert.AreEqual(actual.UserId, expected.UserId);
            Assert.AreEqual(actual.Username, expected.Username);
            Assert.AreEqual(actual.PasswordHash, expected.PasswordHash);
            Assert.AreEqual(actual.Salt, expected.Salt);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Roll back the transaction
            transaction.Dispose();
        }
    }
}
