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
                    GetTransferMenu();
                }
                else if (menuSelection == 3)
                {
                    PendingTransferMenu();
                }
                else if (menuSelection == 4)
                {
                    SendTransferMenu();
                }
                else if (menuSelection == 5)
                {
                    RequestTransferMenu();
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
            ListUsers();
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
        private static void RequestTransferMenu()
        {
            ListUsers();
            Console.WriteLine("Please Enter the userId of the user you would like to request a payment from: ");
            int fromUserId = int.Parse(Console.ReadLine().Trim());

            Console.WriteLine();
            Console.WriteLine("Please enter the amount you would like to request: ");
            //non-integer exception handling with that parse needed
            decimal amount = decimal.Parse(Console.ReadLine().Trim());
            try
            {
                Transfer transfer = transferService.RequestTransfer(amount, fromUserId);
                Console.WriteLine("");
                Console.WriteLine("Request Created!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void GetTransferMenu()
        {
            List<Transfer> transfers = transferService.GetTransfers();
            ListTransfers(transfers);

            string userSelection;
            do
            {
                Console.WriteLine("");
                Console.WriteLine("Would you like to look at the details of a specific transfer (Y/N)?");
                userSelection = Console.ReadLine();

                if (userSelection.ToLower().Trim() == "y" || userSelection.ToLower().Trim() == "yes")
                {
                    // GetTransferById 
                    Console.WriteLine();
                    Console.WriteLine("Enter the transferId number.");

                    try
                    {
                        int transferId = int.Parse(Console.ReadLine());
                        Transfer transfer = transferService.GetTransferById(transferId);

                        Console.WriteLine("");
                        Console.WriteLine($"| Transfer ID: {transfer.TransferId.ToString().PadRight(4)}\n" +
                        $"| Transfer Type: {transfer.TransferTypeDesc.ToString().PadRight(8)}\n" +
                        $"| Transfer Status: {transfer.TransferStatusDesc.ToString().PadRight(5)}\n" +
                        $"| From User : {transfer.FromUsername.ToString().PadRight(4)}\n" +
                        $"| To User : {transfer.ToUsername.ToString().PadRight(4)}\n" +
                        $"| Amount: {transfer.Amount.ToString("C2")}\n");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid Entry. Please try again.");
                    }
                    break;
                }

            } while (userSelection.ToLower() != "n" && userSelection.ToLower() != "no");

        }
        private static void PendingTransferMenu()
        {
            List<Transfer> transfers = transferService.GetPendingTransfers();
            ListTransfers(transfers);
            string userSelection;
            Console.WriteLine("");
            Console.WriteLine("Would you like to approve/reject a specific transfer (Y/N)?");
            userSelection = Console.ReadLine();

            if (userSelection.ToLower().Trim() == "y" || userSelection.ToLower().Trim() == "yes")
            {
                // GetTransferById 
                Console.WriteLine();
                Console.WriteLine("Enter the transferId number.");

                try
                {
                    int transferId = int.Parse(Console.ReadLine());
                    Transfer transfer = transferService.GetTransferById(transferId);

                    Console.WriteLine("");
                    Console.WriteLine($"| Transfer ID: {transfer.TransferId.ToString().PadRight(4)}\n" +
                    $"| Transfer Type: {transfer.TransferTypeDesc.ToString().PadRight(8)}\n" +
                    $"| Transfer Status: {transfer.TransferStatusDesc.ToString().PadRight(5)}\n" +
                    $"| From User : {transfer.FromUsername.ToString().PadRight(4)}\n" +
                    $"| To User : {transfer.ToUsername.ToString().PadRight(4)}\n" +
                    $"| Amount: {transfer.Amount.ToString("C2")}\n");
                    //Prompt for approval
                    Console.WriteLine("");
                    Console.WriteLine("Would you like to Approve or Reject this transfer?");
                    userSelection = Console.ReadLine();
                    if (userSelection.ToLower().Trim() == "a" || userSelection.ToLower().Trim() == "approve")
                    {
                        transferService.ApproveTransfer(transfer);
                        Console.WriteLine("Your money has been sent!");
                    }
                    else if (userSelection.ToLower().Trim() == "r" || userSelection.ToLower().Trim() == "reject")
                    {
                        transferService.RejectTransfer(transfer);
                        Console.WriteLine("Yeah! They don't need your money anyways!");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid Entry. Please try again.");
                }
            }
            
        }
        
        private static void ListTransfers(List<Transfer> transfers)
        {
            int count = 0;
            foreach (Transfer transfer in transfers)
            {
                count++;

                Console.Write($"Transfer ID: {transfer.TransferId.ToString().PadRight(4)}");
                if (UserService.GetUserId() != transfer.FromUserId)
                {
                    Console.Write($"| From: {transfer.FromUsername.ToString().PadRight(12)}");
                }
                if (UserService.GetUserId() != transfer.ToUserId)
                {
                    Console.Write($"| To  : {transfer.ToUsername.ToString().PadRight(12)}");
                }
                Console.Write($"| Amount: {transfer.Amount.ToString("C2")}\n");
            }
        }
        private static void ListUsers()
        {
            List<User> users = generalService.GetUsers();
            Console.WriteLine("Current Users:");
            Console.WriteLine("User ID  | User");
            foreach (User user in users)
            {
                if (user.UserId != UserService.GetUserId())
                {
                    Console.WriteLine($"     {user.UserId.ToString().PadRight(3)} : {user.Username}");
                }
            }
        }
    }
}
