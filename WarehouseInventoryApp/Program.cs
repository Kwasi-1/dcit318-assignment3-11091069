using System;
using System.Collections.Generic;

// Marker interface for inventory items
public interface IInventoryItem
{
  int Id { get; }
  string Name { get; }
  int Quantity { get; set; }
}

// ElectronicItem class
public class ElectronicItem : IInventoryItem
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Quantity { get; set; }
  public string Brand { get; set; }
  public int WarrantyMonths { get; set; }

  public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
  {
    Id = id;
    Name = name;
    Quantity = quantity;
    Brand = brand;
    WarrantyMonths = warrantyMonths;
  }

  public override string ToString() => $"ID: {Id}, Name: {Name}, Brand: {Brand}, Warranty: {WarrantyMonths} months, Quantity: {Quantity}";
}

// GroceryItem class
public class GroceryItem : IInventoryItem
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int Quantity { get; set; }
  public DateTime ExpiryDate { get; set; }

  public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
  {
    Id = id;
    Name = name;
    Quantity = quantity;
    ExpiryDate = expiryDate;
  }

  public override string ToString() => $"ID: {Id}, Name: {Name}, Expiry: {ExpiryDate:yyyy-MM-dd}, Quantity: {Quantity}";
}

// Custom Exceptions
public class DuplicateItemException : Exception
{
  public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
  public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
  public InvalidQuantityException(string message) : base(message) { }
}

// Generic Inventory Repository
public class InventoryRepository<T> where T : IInventoryItem
{
  private Dictionary<int, T> _items = new Dictionary<int, T>();

  public void AddItem(T item)
  {
    if (_items.ContainsKey(item.Id))
      throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
    _items[item.Id] = item;
  }

  public T GetItemById(int id)
  {
    if (!_items.ContainsKey(id))
      throw new ItemNotFoundException($"Item with ID {id} not found.");
    return _items[id];
  }

  public void RemoveItem(int id)
  {
    if (!_items.ContainsKey(id))
      throw new ItemNotFoundException($"Item with ID {id} not found.");
    _items.Remove(id);
  }

  public List<T> GetAllItems() => new List<T>(_items.Values);

  public void UpdateQuantity(int id, int newQuantity)
  {
    if (newQuantity < 0)
      throw new InvalidQuantityException("Quantity cannot be negative.");
    if (!_items.ContainsKey(id))
      throw new ItemNotFoundException($"Item with ID {id} not found.");
    _items[id].Quantity = newQuantity;
  }
}

// WareHouseManager class
public class WareHouseManager
{
  private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
  private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

  public void SeedData()
  {
    // Add electronic items
    _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
    _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));
    _electronics.AddItem(new ElectronicItem(3, "Tablet", 15, "Apple", 18));
    // Add grocery items
    _groceries.AddItem(new GroceryItem(101, "Milk", 30, DateTime.Now.AddDays(7)));
    _groceries.AddItem(new GroceryItem(102, "Bread", 50, DateTime.Now.AddDays(3)));
    _groceries.AddItem(new GroceryItem(103, "Eggs", 100, DateTime.Now.AddDays(14)));
  }

  public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
  {
    foreach (var item in repo.GetAllItems())
    {
      Console.WriteLine(item);
    }
  }

  public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
  {
    try
    {
      var item = repo.GetItemById(id);
      repo.UpdateQuantity(id, item.Quantity + quantity);
      Console.WriteLine($"Stock increased for item ID {id}. New quantity: {item.Quantity + quantity}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error: {ex.Message}");
    }
  }

  public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
  {
    try
    {
      repo.RemoveItem(id);
      Console.WriteLine($"Item with ID {id} removed.");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error: {ex.Message}");
    }
  }

  public void Demo()
  {
    Console.WriteLine("All Grocery Items:");
    PrintAllItems(_groceries);
    Console.WriteLine();
    Console.WriteLine("All Electronic Items:");
    PrintAllItems(_electronics);
    Console.WriteLine();

    // Try to add a duplicate item
    try
    {
      _electronics.AddItem(new ElectronicItem(1, "Monitor", 5, "LG", 24));
    }
    catch (DuplicateItemException ex)
    {
      Console.WriteLine($"Duplicate Error: {ex.Message}");
    }

    // Try to remove a non-existent item
    RemoveItemById(_groceries, 999);

    // Try to update with invalid quantity
    try
    {
      _groceries.UpdateQuantity(101, -5);
    }
    catch (InvalidQuantityException ex)
    {
      Console.WriteLine($"Invalid Quantity Error: {ex.Message}");
    }
  }
}

// Program entry point
class Program
{
  static void Main(string[] args)
  {
    var manager = new WareHouseManager();
    manager.SeedData();
    manager.Demo();
  }
}
