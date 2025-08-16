using System;
using System.Collections.Generic;

// Marker Interface for Inventory Items
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Product Classes
public class ElectronicItem : IInventoryItem
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; set; }
    public string Brand { get; private set; }
    public int WarrantyMonths { get; private set; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"Electronic Item [ID: {Id}, Name: {Name}, Quantity: {Quantity}, Brand: {Brand}, Warranty: {WarrantyMonths} months]";
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; private set; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"Grocery Item [ID: {Id}, Name: {Name}, Quantity: {Quantity}, Expiry Date: {ExpiryDate.ToShortDateString()}]";
    }
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
    private Dictionary<int, T> _items;

    public InventoryRepository()
    {
        _items = new Dictionary<int, T>();
    }

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists in the inventory.");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} was not found in the inventory.");
        }
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} was not found in the inventory.");
        }
        _items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException($"Quantity cannot be negative. Attempted to set quantity to {newQuantity}.");
        }
        
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} was not found in the inventory.");
        }
        
        _items[id].Quantity = newQuantity;
    }
}

// Warehouse Manager Class
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics;
    private InventoryRepository<GroceryItem> _groceries;

    public WareHouseManager()
    {
        _electronics = new InventoryRepository<ElectronicItem>();
        _groceries = new InventoryRepository<GroceryItem>();
    }

    public void SeedData()
    {
        // Add Electronic Items
        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Tablet", 15, "Apple", 12));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding electronic items: {ex.Message}");
        }

        // Add Grocery Items
        try
        {
            _groceries.AddItem(new GroceryItem(101, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(102, "Bread", 30, DateTime.Now.AddDays(3)));
            _groceries.AddItem(new GroceryItem(103, "Apples", 100, DateTime.Now.AddDays(10)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding grocery items: {ex.Message}");
        }

        Console.WriteLine("Seed data has been loaded successfully.\n");
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        if (items.Count == 0)
        {
            Console.WriteLine("No items found in the repository.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
        Console.WriteLine();
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            int newQuantity = item.Quantity + quantity;
            repo.UpdateQuantity(id, newQuantity);
            Console.WriteLine($"Successfully increased stock for item ID {id}. New quantity: {newQuantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Successfully removed item with ID {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    // Public properties to access repositories for testing
    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;
}

// Main Application
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Warehouse Inventory Management System ===\n");

        // i. Instantiate WareHouseManager
        WareHouseManager warehouse = new WareHouseManager();

        // ii. Call SeedData()
        warehouse.SeedData();

        // iii. Print all grocery items
        Console.WriteLine("=== All Grocery Items ===");
        warehouse.PrintAllItems(warehouse.Groceries);

        // iv. Print all electronic items
        Console.WriteLine("=== All Electronic Items ===");
        warehouse.PrintAllItems(warehouse.Electronics);

        // v. Try operations that should trigger exceptions
        Console.WriteLine("=== Testing Exception Handling ===");

        // Test 1: Add a duplicate item
        Console.WriteLine("1. Attempting to add duplicate item (ID: 1):");
        try
        {
            warehouse.Electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 18));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Caught DuplicateItemException: {ex.Message}");
        }
        Console.WriteLine();

        // Test 2: Remove a non-existent item
        Console.WriteLine("2. Attempting to remove non-existent item (ID: 999):");
        warehouse.RemoveItemById(warehouse.Electronics, 999);
        Console.WriteLine();

        // Test 3: Update with invalid quantity
        Console.WriteLine("3. Attempting to update quantity with negative value:");
        try
        {
            warehouse.Electronics.UpdateQuantity(1, -10);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Caught InvalidQuantityException: {ex.Message}");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Caught ItemNotFoundException: {ex.Message}");
        }
        Console.WriteLine();

        // Additional demonstrations
        Console.WriteLine("=== Additional Operations ===");
        
        // Successfully increase stock
        Console.WriteLine("4. Successfully increasing stock for existing item:");
        warehouse.IncreaseStock(warehouse.Groceries, 101, 20);
        Console.WriteLine();

        // Show updated inventory
        Console.WriteLine("=== Updated Grocery Items ===");
        warehouse.PrintAllItems(warehouse.Groceries);

        Console.WriteLine("=== System Demo Complete ===");
        Console.ReadLine(); // Keep console open
    }
}