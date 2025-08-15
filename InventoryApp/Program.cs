using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface for logging
public interface IInventoryEntity
{
  int Id { get; }
}

// Immutable InventoryItem record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
  private List<T> _log = new List<T>();
  private string _filePath;

  public InventoryLogger(string filePath)
  {
    _filePath = filePath;
  }

  public void Add(T item) => _log.Add(item);
  public List<T> GetAll() => new List<T>(_log);

  public void SaveToFile()
  {
    try
    {
      var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText(_filePath, json);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error saving to file: {ex.Message}");
    }
  }

  public void LoadFromFile()
  {
    try
    {
      if (!File.Exists(_filePath))
      {
        Console.WriteLine("No data file found. Starting with empty log.");
        _log.Clear();
        return;
      }
      var json = File.ReadAllText(_filePath);
      var items = JsonSerializer.Deserialize<List<T>>(json);
      _log = items ?? new List<T>();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error loading from file: {ex.Message}");
    }
  }
}

// InventoryApp integration layer
public class InventoryApp
{
  private InventoryLogger<InventoryItem> _logger;
  private string _dataFile = "inventory.json";

  public InventoryApp()
  {
    _logger = new InventoryLogger<InventoryItem>(_dataFile);
  }

  public void SeedSampleData()
  {
    _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now.AddDays(-10)));
    _logger.Add(new InventoryItem(2, "Printer", 5, DateTime.Now.AddDays(-8)));
    _logger.Add(new InventoryItem(3, "Desk Chair", 20, DateTime.Now.AddDays(-5)));
    _logger.Add(new InventoryItem(4, "Monitor", 7, DateTime.Now.AddDays(-2)));
    _logger.Add(new InventoryItem(5, "Keyboard", 15, DateTime.Now));
  }

  public void SaveData() => _logger.SaveToFile();
  public void LoadData() => _logger.LoadFromFile();

  public void PrintAllItems()
  {
    var items = _logger.GetAll();
    if (items.Count == 0)
    {
      Console.WriteLine("No inventory items found.");
      return;
    }
    foreach (var item in items)
    {
      Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:yyyy-MM-dd}");
    }
  }

  public void ClearMemory()
  {
    // Simulate clearing memory by creating a new logger instance
    _logger = new InventoryLogger<InventoryItem>(_dataFile);
  }
}

// Program entry point
class Program
{
  static void Main(string[] args)
  {
    var app = new InventoryApp();
    app.SeedSampleData();
    app.SaveData();
    app.ClearMemory(); // Simulate a new session
    app.LoadData();
    app.PrintAllItems();
  }
}
