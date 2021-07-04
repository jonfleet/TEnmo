using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {

        private static readonly AuthService authService = new AuthService();
        private static readonly TransferService transferService = new TransferService();
        private static readonly GeneralService generalService = new GeneralService();
        private static readonly ConsoleService consoleService = new ConsoleService();

        public void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = authService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = authService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    Balance balance = generalService.ReturnBalance();
                    Console.WriteLine();
                    Console.WriteLine($"Your Balance is currently {balance.PrimaryBalance.ToString("C2")}");
                }
                else if (menuSelection == 2)
                {
                    List<Transfer> transfers = transferService.GetTransfers();
                    int count = 0;
                    foreach(Transfer transfer in transfers)
                    {
                        count++;
                        Console.WriteLine($"Transfer ID: {count.ToString().PadRight(4)}| Transfer Type: {transfer.TransferTypeDesc.ToString().PadRight(8)}" +
                            $"| Transfer Status: {transfer.TransferStatusDesc.ToString().PadRight(5)}| From User ID:{transfer.FromUserId.ToString().PadRight(4)}" +
                            $"| To User ID:{transfer.ToUserId.ToString().PadRight(4)}| {transfer.Amount.ToString("C2")}");
                    }
                    //Console.WriteLine("");
                    //Console.WriteLine("Would you like to look at the details of a specific transfer?");
                    //Console.WriteLine("If not, please enter 0");
                    //int userSelection = -1;
                    //while (userSelection != 0)
                    //{
                    //    if (!int.TryParse(Console.ReadLine(), out userSelection))
                    //    {
                    //        Console.WriteLine("That wasn't a number! please use the number pad!!");
                    //    }
                    //    else if (userSelection <= transfers.Count - 1)
                    //    {
                            
                    //    }
                    //}
                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    SendTransferMenu();
                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    consoleService.Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }

        private static void SendTransferMenu()
        {
            List<User> users = generalService.GetUsers();
            Console.WriteLine("Current Users:");
            Console.WriteLine("User ID | User");
            foreach (User user in users)
            {
                if (user.UserId != UserService.GetUserId())
                {
                    Console.WriteLine($"     {user.UserId.ToString().PadRight(3)} : {user.Username}");
                }
            }

            Console.WriteLine("Please Enter the userId of the user you would like to send a payment to: ");
            int toUserId = int.Parse(Console.ReadLine().Trim());

            Console.WriteLine();
            Console.WriteLine("Please enter the amount you would like to send: ");
            decimal amount = decimal.Parse(Console.ReadLine().Trim());
            try
            {
                Transfer transfer = transferService.SendTransfer(amount, toUserId);
                Console.WriteLine("");
                Console.WriteLine("Transaction Completed!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }       
    }
}
