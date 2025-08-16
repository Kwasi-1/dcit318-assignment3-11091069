using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // a. Transaction record to represent financial data
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b. Interface for transaction processing
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c. Concrete implementations of ITransactionProcessor
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Bank Transfer: Processing ${transaction.Amount:F2} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Mobile Money: Processing ${transaction.Amount:F2} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Crypto Wallet: Processing ${transaction.Amount:F2} for {transaction.Category}");
        }
    }

    // d. Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: ${Balance:F2}");
        }
    }

    // e. Sealed SavingsAccount class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) 
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction applied. Updated balance: ${Balance:F2}");
            }
        }
    }

    // f. FinanceApp class to integrate and simulate the system
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            Console.WriteLine("=== Finance Management System ===\n");

            // i. Instantiate a SavingsAccount
            var savingsAccount = new SavingsAccount("SAV-001", 1000m);
            Console.WriteLine($"Created Savings Account: {savingsAccount.AccountNumber}");
            Console.WriteLine($"Initial Balance: ${savingsAccount.Balance:F2}\n");

            // ii. Create three Transaction records
            var transaction1 = new Transaction(1, DateTime.Now, 150.75m, "Groceries");
            var transaction2 = new Transaction(2, DateTime.Now.AddHours(-2), 89.50m, "Utilities");
            var transaction3 = new Transaction(3, DateTime.Now.AddHours(-5), 45.25m, "Entertainment");

            // iii. Create processors and process each transaction
            var mobileMoneyProcessor = new MobileMoneyProcessor();
            var bankTransferProcessor = new BankTransferProcessor();
            var cryptoWalletProcessor = new CryptoWalletProcessor();

            Console.WriteLine("Processing Transactions:\n");

            // Process Transaction 1 with MobileMoneyProcessor
            Console.WriteLine($"Transaction 1 - ID: {transaction1.Id}, Date: {transaction1.Date:yyyy-MM-dd HH:mm}");
            mobileMoneyProcessor.Process(transaction1);
            savingsAccount.ApplyTransaction(transaction1);
            Console.WriteLine();

            // Process Transaction 2 with BankTransferProcessor
            Console.WriteLine($"Transaction 2 - ID: {transaction2.Id}, Date: {transaction2.Date:yyyy-MM-dd HH:mm}");
            bankTransferProcessor.Process(transaction2);
            savingsAccount.ApplyTransaction(transaction2);
            Console.WriteLine();

            // Process Transaction 3 with CryptoWalletProcessor
            Console.WriteLine($"Transaction 3 - ID: {transaction3.Id}, Date: {transaction3.Date:yyyy-MM-dd HH:mm}");
            cryptoWalletProcessor.Process(transaction3);
            savingsAccount.ApplyTransaction(transaction3);
            Console.WriteLine();

            // v. Add all transactions to _transactions
            _transactions.Add(transaction1);
            _transactions.Add(transaction2);
            _transactions.Add(transaction3);

            // Display summary
            Console.WriteLine("=== Transaction Summary ===");
            Console.WriteLine($"Total transactions processed: {_transactions.Count}");
            Console.WriteLine($"Final account balance: ${savingsAccount.Balance:F2}");
            
            // Test insufficient funds scenario
            Console.WriteLine("\n=== Testing Insufficient Funds ===");
            var largeTransaction = new Transaction(4, DateTime.Now, 800m, "Large Purchase");
            Console.WriteLine($"Attempting transaction of ${largeTransaction.Amount:F2}");
            savingsAccount.ApplyTransaction(largeTransaction);
        }
    }

    // Main program entry point
    public class Program
    {
        public static void Main(string[] args)
        {
            var financeApp = new FinanceApp();
            financeApp.Run();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}