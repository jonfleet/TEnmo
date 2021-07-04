using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class GeneralService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly RestClient client = new RestClient();

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
                return balance;
            }
        }

        public List<User> GetUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new HttpRequestException("An error occured while communicating with the server");
            }
            else if (!response.IsSuccessful)
            {
                throw new HttpRequestException("Response was unsuccessful: " + (int)response.StatusCode + " " + response.StatusDescription);
            }
            else
            {
                return response.Data;
            }
        }
    }
}
