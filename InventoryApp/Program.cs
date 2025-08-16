using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace InventorySystem
{
    // a. Define an Immutable Inventory Record using positional syntax
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // b. Define Marker Interface for Logging
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // c. Create a Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log;
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _log = new List<T>();
            _filePath = filePath;
        }

        // Add item to log
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            
            _log.Add(item);
            Console.WriteLine($"Added item with ID {item.Id} to inventory log.");
        }

        // Return all items in the log
        public List<T> GetAll()
        {
            return new List<T>(_log); // Return a copy to maintain immutability
        }

        // d. & e. Serialize all items to a file with proper exception handling
        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string jsonData = JsonSerializer.Serialize(_log, options);
                
                // Using statement for automatic resource disposal
                using (var writer = new StreamWriter(_filePath))
                {
                    writer.Write(jsonData);
                }
                
                Console.WriteLine($"Successfully saved {_log.Count} items to {_filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied when saving to file: {ex.Message}");
                throw;
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory not found: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error occurred while saving: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during save operation: {ex.Message}");
                throw;
            }
        }

        // Deserialize items from file with proper exception handling
        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"File {_filePath} does not exist. Starting with empty inventory.");
                    _log = new List<T>();
                    return;
                }

                string jsonData;
                
                // Using statement for automatic resource disposal
                using (var reader = new StreamReader(_filePath))
                {
                    jsonData = reader.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    Console.WriteLine("File is empty. Starting with empty inventory.");
                    _log = new List<T>();
                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var loadedItems = JsonSerializer.Deserialize<List<T>>(jsonData, options);
                _log = loadedItems ?? new List<T>();
                
                Console.WriteLine($"Successfully loaded {_log.Count} items from {_filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied when loading file: {ex.Message}");
                throw;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                _log = new List<T>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON data: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error occurred while loading: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during load operation: {ex.Message}");
                throw;
            }
        }

        // Clear the in-memory log (useful for testing)
        public void ClearMemory()
        {
            _log.Clear();
            Console.WriteLine("Memory cleared - simulating new session.");
        }
    }

    // f. Integration Layer – InventoryApp
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp()
        {
            // Use a file in the current directory
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "inventory_data.json");
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        // Seed sample data
        public void SeedSampleData()
        {
            Console.WriteLine("\n=== Seeding Sample Data ===");
            
            var sampleItems = new List<InventoryItem>
            {
                new InventoryItem(1, "Laptop", 15, DateTime.Now.AddDays(-30)),
                new InventoryItem(2, "Wireless Mouse", 50, DateTime.Now.AddDays(-25)),
                new InventoryItem(3, "Mechanical Keyboard", 25, DateTime.Now.AddDays(-20)),
                new InventoryItem(4, "Monitor", 12, DateTime.Now.AddDays(-15)),
                new InventoryItem(5, "USB Cable", 100, DateTime.Now.AddDays(-10))
            };

            foreach (var item in sampleItems)
            {
                _logger.Add(item);
            }
        }

        // Save data to file
        public void SaveData()
        {
            Console.WriteLine("\n=== Saving Data to File ===");
            _logger.SaveToFile();
        }

        // Load data from file
        public void LoadData()
        {
            Console.WriteLine("\n=== Loading Data from File ===");
            _logger.LoadFromFile();
        }

        // Print all items
        public void PrintAllItems()
        {
            Console.WriteLine("\n=== Current Inventory Items ===");
            var items = _logger.GetAll();
            
            if (!items.Any())
            {
                Console.WriteLine("No items found in inventory.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Name",-20} {"Quantity",-10} {"Date Added",-15}");
            Console.WriteLine(new string('-', 55));
            
            foreach (var item in items.OrderBy(x => x.Id))
            {
                Console.WriteLine($"{item.Id,-5} {item.Name,-20} {item.Quantity,-10} {item.DateAdded:yyyy-MM-dd}");
            }
            
            Console.WriteLine($"\nTotal items: {items.Count}");
            Console.WriteLine($"Total quantity: {items.Sum(x => x.Quantity)}");
        }

        // Clear memory for simulation
        public void ClearMemoryForSimulation()
        {
            Console.WriteLine("\n=== Clearing Memory (Simulating New Session) ===");
            _logger.ClearMemory();
        }

        // Get statistics about the inventory
        public void PrintInventoryStatistics()
        {
            Console.WriteLine("\n=== Inventory Statistics ===");
            var items = _logger.GetAll();
            
            if (!items.Any())
            {
                Console.WriteLine("No items to analyze.");
                return;
            }

            var totalItems = items.Count;
            var totalQuantity = items.Sum(x => x.Quantity);
            var avgQuantity = items.Average(x => x.Quantity);
            var oldestItem = items.OrderBy(x => x.DateAdded).First();
            var newestItem = items.OrderByDescending(x => x.DateAdded).First();

            Console.WriteLine($"Total unique items: {totalItems}");
            Console.WriteLine($"Total quantity across all items: {totalQuantity}");
            Console.WriteLine($"Average quantity per item: {avgQuantity:F2}");
            Console.WriteLine($"Oldest item: {oldestItem.Name} (added {oldestItem.DateAdded:yyyy-MM-dd})");
            Console.WriteLine($"Newest item: {newestItem.Name} (added {newestItem.DateAdded:yyyy-MM-dd})");
        }
    }

    // g. Main Application Flow
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== Inventory Management System ===");
                Console.WriteLine("Demonstrating C# Records, Generics, and File Operations\n");

                // Create an instance of InventoryApp
                var inventoryApp = new InventoryApp();

                // Call SeedSampleData()
                inventoryApp.SeedSampleData();

                // Print initial data
                inventoryApp.PrintAllItems();
                inventoryApp.PrintInventoryStatistics();

                // Call SaveData() to persist to disk
                inventoryApp.SaveData();

                // Clear memory and simulate a new session
                inventoryApp.ClearMemoryForSimulation();

                // Verify memory is cleared
                Console.WriteLine("\n=== Verifying Memory is Cleared ===");
                inventoryApp.PrintAllItems();

                // Call LoadData() to read from file
                inventoryApp.LoadData();

                // Call PrintAllItems() to confirm data was recovered
                Console.WriteLine("\n=== Data Successfully Recovered ===");
                inventoryApp.PrintAllItems();
                inventoryApp.PrintInventoryStatistics();

                Console.WriteLine("\n=== System Demonstration Complete ===");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFatal error in application: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}