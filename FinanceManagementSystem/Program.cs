
using System;
using System.Collections.Generic;

// Record to represent a transaction
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface for transaction processing
public interface ITransactionProcessor
{
  void Process(Transaction transaction);
}

// Concrete processor implementations
public class BankTransferProcessor : ITransactionProcessor
{
  public void Process(Transaction transaction)
  {
    Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
  }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
  public void Process(Transaction transaction)
  {
    Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
  }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
  public void Process(Transaction transaction)
  {
    Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
  }
}

// Base Account class
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
  }
}

// Sealed SavingsAccount class
public sealed class SavingsAccount : Account
{
  public SavingsAccount(string accountNumber, decimal initialBalance)
    : base(accountNumber, initialBalance) { }

  public override void ApplyTransaction(Transaction transaction)
  {
    if (transaction.Amount > Balance)
    {
      Console.WriteLine("Insufficient funds");
    }
    else
    {
      Balance -= transaction.Amount;
      Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
    }
  }
}

// Main FinanceApp class
public class FinanceApp
{
  private List<Transaction> _transactions = new();

  public void Run()
  {
    var account = new SavingsAccount("ACC123", 1000m);

    var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
    var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
    var t3 = new Transaction(3, DateTime.Now, 300m, "Entertainment");

    ITransactionProcessor[] processors = {
      new MobileMoneyProcessor(),
      new BankTransferProcessor(),
      new CryptoWalletProcessor()
    };

    Transaction[] transactions = { t1, t2, t3 };

    for (int i = 0; i < transactions.Length; i++)
    {
      processors[i].Process(transactions[i]);
      account.ApplyTransaction(transactions[i]);
      _transactions.Add(transactions[i]);
    }
  }
}

// Program entry point
class Program
{
  static void Main(string[] args)
  {
    var app = new FinanceApp();
    app.Run();
  }
}
