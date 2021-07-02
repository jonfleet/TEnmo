using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class UserSqlDAO : IUserDAO
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public UserSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }

        public List<User> GetUsers()
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM users", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            User u = GetUserFromReader(reader);
                            returnUsers.Add(u);
                        }

                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO users (username, password_hash, salt) VALUES (@username, @password_hash, @salt)", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("INSERT INTO accounts (user_id, balance) VALUES (@userid, @startBalance)", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return GetUser(username);
        }


        public List<Transfer> GetTransfers(string username)
        {
            List<Transfer> returnTransactions = new List<Transfer>();

            try
            {
                using (SqlConnection conny = new SqlConnection(connectionString))
                {
                    conny.Open();
                    SqlCommand cmd = new SqlCommand("select * from transfers as t " +
                        "join accounts as a " +
                        "on a.account_id = t.account_from " +
                        "join users as u " +
                        "on u.user_id = a.user_id " +
                        "where " +
                        "(t.account_from = (select user_id from users where username = @username)) " +
                        "or(t.account_to = (select user_id from users where username = @username))", conny);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Transfer t = GetTransferFromReader(reader);
                            returnTransactions.Add(t);
                        }
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return returnTransactions;
        }

        public Transfer GetTransferById(int transferId, string username)
        {
            Transfer transfer = null;
            try
            {
                using (SqlConnection conny = new SqlConnection(connectionString))
                {
                    conny.Open();
                    SqlCommand cmd = new SqlCommand("select * from transfers as t " +
                        "join accounts as a " +
                        "on a.account_id = t.account_from " +
                        "join users as u " +
                        "on u.user_id = a.user_id " +
                        "where (t.transfer_id = @transferId) and" +
                        "((t.account_from = (select user_id from users where username = @username))" +
                        "or(t.account_to = (select user_id from users where username = @username)))", conny);

                    cmd.Parameters.AddWithValue( "@transferId", transferId);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if(reader.Read())
                    {
                        transfer = GetTransferFromReader(reader);
                    }
                }

            }
            catch (Exception e)
            {

                throw e;
            }
            return transfer;
        }

        public Transfer CreateTransfer(Transfer transfer, string transferType)
        {
            try
            {
                using (SqlConnection conny = new SqlConnection(connectionString))
                {
                    conny.Open();

                    // Create new Transaction
                    SqlCommand cmd = new SqlCommand("Insert into transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "OUTPUT INSERTED.transfer_id " +
                        "values " +
                        "(" +
                        "   (select transfer_type_id from transfer_types where transfer_type_desc = @transferType), " +
                        "   (select transfer_status_id from transfer_statuses where transfer_status_desc = 'Approved'), " +
                        "   @fromUser, " +
                        "   @toUser, " +
                        "   @amount" +
                        ");", conny);
                    cmd.Parameters.AddWithValue("@transferType", transferType);
                    cmd.Parameters.AddWithValue("@transferStatus", "Pending");
                    cmd.Parameters.AddWithValue("@fromUser", transfer.FromUserId);
                    cmd.Parameters.AddWithValue("@toUser", transfer.ToUserId);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);

                    transfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Decrement FromUserBalance
                    SqlCommand decrementCommand = new SqlCommand(
                        "IF((select balance from accounts where user_id = @fromUser) > @withdrawAmount) " +
                        "BEGIN " +
                            "Update accounts " +
                            "set balance = balance - @withdrawAmount " +
                            "where user_id = @fromUser; " +
                         "END", conny);
                    decrementCommand.Parameters.AddWithValue("@withdrawAmount", transfer.Amount);
                    decrementCommand.Parameters.AddWithValue("@fromUser", transfer.FromUserId);

                    decrementCommand.ExecuteScalar();
                    
                    // Increment ToUserBalance
                    SqlCommand incrementCommand = new SqlCommand(
                        "Update accounts " +
                        "set balance = balance + @depositAmount " +
                        "where user_id = @toUser;", conny
                    );
                    incrementCommand.Parameters.AddWithValue("@depositAmount", transfer.Amount);
                    incrementCommand.Parameters.AddWithValue("@toUser", transfer.ToUserId);
                    
                    incrementCommand.ExecuteScalar();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return transfer;
        }

        public Balance GetBalance(int userId)
        {
            Balance userBalance = new Balance();

            try
            {
                using (SqlConnection conny = new SqlConnection(connectionString))
                {
                    conny.Open();
                    SqlCommand cmd = new SqlCommand("select * from accounts as a where a.user_id = @userId;", conny);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userBalance.AccountId = Convert.ToInt32(reader["account_id"]);
                        userBalance.UserId = Convert.ToInt32(reader["user_id"]);
                        userBalance.PrimaryBalance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return userBalance;
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
            };

            return u;
        }
        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                FromUserId = Convert.ToInt32(reader["account_from"]),
                ToUserId = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"]),
            };
            return t;
        }
        
    }
}
