# DCIT 318 – Programming II Assignment 3

## Overview

This repository contains solutions to all five questions of the DCIT 318 Assignment 3. Each question is implemented as a separate C# console application, organized by folder. All code for each question is in a single `Program.cs` file for easy review and submission.

---

## Folder Structure

```
finance management system/FinanceManagementSystem/         # Question 1
healthcare record management system/HealthcareSystemApp/   # Question 2
inventory management system/InventoryApp/                  # Question 5
student grading system file processing/StudentGradingApp/  # Question 4
warehouse inventory management system/WarehouseInventoryApp/ # Question 3
```

---

## How to Run Each App

### Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/download) installed
- Use a terminal or command prompt

### General Steps

1. Open a terminal and navigate to the folder of the app you want to run (e.g., `cd "finance management system/FinanceManagementSystem"`).
2. Run the app with:
   ```
   dotnet run
   ```

---

## Question 1: Finance Management System

- **Location:** `finance management system/FinanceManagementSystem/`
- **Description:** Tracks transactions, enforces data integrity, and supports multiple transaction types and account behaviors using interfaces, records, and sealed classes.
- **How to Run:**
  1. Navigate to the folder.
  2. Run `dotnet run`.

---

## Question 2: Healthcare System

- **Location:** `healthcare record management system/HealthcareSystemApp/`
- **Description:** Manages patient records and prescriptions using collections and generics.
- **How to Run:**
  1. Navigate to the folder.
  2. Run `dotnet run`.

---

## Question 3: Warehouse Inventory Management System

- **Location:** `warehouse inventory management system/WarehouseInventoryApp/`
- **Description:** Manages electronic and grocery inventory with generics, collections, and custom exception handling.
- **How to Run:**
  1. Navigate to the folder.
  2. Run `dotnet run`.

---

## Question 4: Student Grading System (File Processing)

- **Location:** `student grading system file processing/StudentGradingApp/`
- **Description:** Reads student data from a text file, assigns grades, and writes a summary report, handling file and data errors.
- **How to Run:**
  1. Place a file named `students.txt` in the same folder. Each line: `ID, Full Name, Score` (e.g., `101, Alice Smith, 84`).
  2. Run `dotnet run`.
  3. Check `report.txt` for the output.

---

## Question 5: Inventory Records with Records, Generics, and File Operations

- **Location:** `inventory management system/InventoryApp/`
- **Description:** Uses C# records for immutable inventory items, generics for logging, and file operations for saving/loading data.
- **How to Run:**
  1. Navigate to the folder.
  2. Run `dotnet run`.
  3. The app will create and use `inventory.json` for data storage.

---

## Notes

- All code is in a single file per app for easy review.
- Make sure to use the correct folder for each question.
- If you encounter errors, check that you are in the correct directory and that any required input files (like `students.txt`) are present.

---

**Author:** Afful Nana Kwasi Obeng

**Date:** 15th August 2025
