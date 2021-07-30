using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Exceptions;
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


        public List<Transfer> GetTransfers(int userId)
        {
            List<Transfer> returnTransfers = new List<Transfer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount, u_to.username as to_username, u_from.username as from_username " +
                        "from transfers as t " +
                        "join accounts as a " +
                        "on a.account_id = t.account_from " +
                        "join users as u_from " +
                        "on u_from.user_id = t.account_from " +
                        "join users as u_to " +
                        "on u_to.user_id = t.account_to " +
                        "where " +
                        "(t.account_from = (select user_id from users where user_id = @userId)) " +
                        "or(t.account_to = (select user_id from users where user_id = @userId))", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Transfer t = GetTransferFromReader(reader);
                            returnTransfers.Add(t);
                        }
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return returnTransfers;
        }

        public Transfer GetTransferById(int userId, int transferId)
        {
            Transfer transfer = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount, u_to.username as to_username, u_from.username as from_username " +
                        "from transfers as t " +
                        "join accounts as a " +
                        "on a.account_id = t.account_from " +
                        "join users as u_from " +
                        "on u_from.user_id = t.account_from " +
                        "join users as u_to " +
                        "on u_to.user_id = t.account_to " +
                        "where " +
                        "(t.transfer_id = @transferId) and" +
                        "((t.account_from = (select user_id from users where user_id = @userId))" +
                        "or(t.account_to = (select user_id from users where user_id = @userId)))", conn);

                    cmd.Parameters.AddWithValue( "@transferId", transferId);
                    cmd.Parameters.AddWithValue("@userId", userId);
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
        public List<Transfer> GetPendingTransfersForUser(int userId)
        {
            List<Transfer> pendingTransfers = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("select transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount, u_to.username as to_username, u_from.username as from_username from transfers as t " +
                    "join accounts as a on a.account_id = t.account_from " +
                    "join users as u_from on u_from.user_id = t.account_from " +
                    "join users as u_to on u_to.user_id = t.account_to " +
                    "where (t.transfer_status_id = (select transfer_status_id from transfer_statuses where transfer_status_desc = 'Pending')) and " +
                    "(t.account_from = (select user_id from users where user_id = @userId));", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Transfer t = GetTransferFromReader(reader);
                            pendingTransfers.Add(t);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return pendingTransfers;
        }

        public Transfer CreateTransfer(Transfer transfer, string transferType)
        {
            if(transfer.Amount < 0)
            {
                throw new Exception();
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();                   

                    // Create new Transaction
                    SqlCommand cmd = new SqlCommand("Insert into transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "OUTPUT INSERTED.transfer_id, INSERTED.transfer_status_id, INSERTED.transfer_type_id " +
                        "values " +
                        "(" +
                        "   (select transfer_type_id from transfer_types where transfer_type_desc = @transferType), " +
                        "   (select transfer_status_id from transfer_statuses where transfer_status_desc = 'Pending'), " +
                        "   @fromUser, " +
                        "   @toUser, " +
                        "   @amount" +
                        ");", conn);
                    cmd.Parameters.AddWithValue("@transferType", transferType);
                    cmd.Parameters.AddWithValue("@fromUser", transfer.FromUserId);
                    cmd.Parameters.AddWithValue("@toUser", transfer.ToUserId);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);

                    SqlDataReader newReader = cmd.ExecuteReader();
                    while (newReader.Read())
                    {
                        transfer.TransferId = Convert.ToInt32(newReader["transfer_id"]);
                        transfer.TransferStatusId = Convert.ToInt32(newReader["transfer_status_id"]);
                        transfer.TransferTypeId = Convert.ToInt32(newReader["transfer_type_id"]);
                    }
                    
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return transfer;
        }
        public Transfer ApproveTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Determine if Sufficient Balance is available
                    SqlCommand getBalance = new SqlCommand("select balance from accounts where user_id = @fromUser", conn);
                    getBalance.Parameters.AddWithValue("@fromUser", transfer.FromUserId);
                    SqlDataReader reader = getBalance.ExecuteReader();

                    decimal currentBalance = 0;
                    if (reader.Read())
                    {
                        currentBalance = Convert.ToDecimal(reader["balance"]);
                    }
                    reader.Close();
                    // Throw Error if balance is not enough to complete transfer
                    if (currentBalance <= transfer.Amount)
                    {
                        conn.Close();
                        throw new InsufficientBalanceException();
                    }


                    // Decrement FromUserBalance
                    SqlCommand decrementCommand = new SqlCommand(
                        "IF((select balance from accounts where user_id = @fromUser) > @withdrawAmount) " +
                        "BEGIN " +
                            "Update accounts " +
                            "set balance = balance - @withdrawAmount " +
                            "where user_id = @fromUser; " +
                         "END", conn);
                    decrementCommand.Parameters.AddWithValue("@withdrawAmount", transfer.Amount);
                    decrementCommand.Parameters.AddWithValue("@fromUser", transfer.FromUserId);

                    decrementCommand.ExecuteScalar();

                    // Increment ToUserBalance
                    SqlCommand incrementCommand = new SqlCommand(
                        "Update accounts " +
                        "set balance = balance + @depositAmount " +
                        "where user_id = @toUser;", conn
                    );
                    incrementCommand.Parameters.AddWithValue("@depositAmount", transfer.Amount);
                    incrementCommand.Parameters.AddWithValue("@toUser", transfer.ToUserId);

                    incrementCommand.ExecuteScalar();

                    //Update Transaction to "Approved"
                    SqlCommand cmd = new SqlCommand("update transfers set transfer_status_id = " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = 'Approved') " +
                        "OUTPUT inserted.transfer_status_id " +
                        "where transfer_id = @transferId;", conn);
                    cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);
                    //cmd.Parameters.AddWithValue("@transferType", transferType);

                    SqlDataReader newReader = cmd.ExecuteReader();
                    if (newReader.Read())
                    {
                        transfer.TransferStatusId = Convert.ToInt32(newReader["transfer_status_id"]);
                        
                    } 
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return transfer;
        }
        public Transfer RejectTransfer(Transfer transfer)
        {
            try
            {
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    //Update Transaction to "Rejected"
                    SqlCommand cmd = new SqlCommand("update transfers set transfer_status_id = " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = 'Rejected') " +
                        "OUTPUT inserted.transfer_status_id " +
                        "where transfer_id = @transferId;", conn);
                    cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        transfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
                    }
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
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from accounts as a where a.user_id = @userId;", conn);
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
            Transfer transfer = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                FromUserId = Convert.ToInt32(reader["account_from"]),
                ToUserId = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"]),
                ToUsername = Convert.ToString(reader["to_username"]),
                FromUsername = Convert.ToString(reader["from_username"])
            };
            return transfer;
        }
        

    }
}
