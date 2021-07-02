﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string TRANSFER_BASE_URL = "https://localhost:44315/transfer/";
        private readonly RestClient client = new RestClient();


        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>
        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int auctionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return auctionId;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }
        public Balance ReturnBalance()
        {
            RestRequest request = new RestRequest(API_BASE_URL + $"user/{UserService.GetUserId()}/balance");
            IRestResponse<Balance> response = client.Get<Balance>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new HttpRequestException("An error occurred communication with the server");
            }
            else if (!response.IsSuccessful)
            {
                throw new HttpRequestException("Response was unsuccessful: " + (int)response.StatusCode + " " + response.StatusDescription);
            }
            else
            {
                Balance balance = response.Data;
                Console.WriteLine();
                Console.WriteLine($"Your Balance is currently {response.Data.PrimaryBalance}");
                return balance;
            }
        }

        public List<Transfer> GetTransfers()
        {
            
            RestRequest request = new RestRequest(API_BASE_URL + $"user/{UserService.GetUserId()}/transfer");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new HttpRequestException("An error occurred communication with the server");
            }
            else if (!response.IsSuccessful)
            {
                throw new HttpRequestException("Response was unsuccessful: " + (int)response.StatusCode + " " + response.StatusDescription);
            }
            else
            {
                List<Transfer> transfers = response.Data;
                return transfers;
            }
        }

        public Transfer SendTransfer(decimal amount, int toUserId)
        {
            RestRequest request = new RestRequest(TRANSFER_BASE_URL + "send");
            Transfer transfer = new Transfer();
            transfer.FromUserId = UserService.GetUserId();
            transfer.Amount = amount;
            transfer.ToUserId = toUserId;
            request.AddJsonBody(transfer);

            IRestResponse<Transfer> response = client.Post<Transfer>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new HttpRequestException("An error occurred while communicating with the server");
            }
            else if (!response.IsSuccessful)
            {
                throw new HttpRequestException("Response was unsuccessful: " + (int)response.StatusCode + " " + response.StatusDescription);
            }
            else
            {
                return null;
            }
        }

        public List<User> GetUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);
            if(response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new HttpRequestException("An error occured while communicating with the server");
            } else if (!response.IsSuccessful)
            {
                throw new HttpRequestException("Response was unsuccessful: " + (int)response.StatusCode + " " + response.StatusDescription);
            } else
            {
                return response.Data;
            }
        }        
    }
}
