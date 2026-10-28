
using Microsoft.Data.SqlClient;
using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;
using PharmacyInventoryManagement.Services;



// Initialize repositories and services
var medicineRepository = new MedicineRepository();


var medicineService = new MedicineService(medicineRepository);


while (true)
{
    Console.WriteLine("\n===== PHARMACY MANAGEMENT =====");
    Console.WriteLine("1. Add Medicine");

    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                {
                    // Display available suppliers
                    var suppliers = supplierService.GetAll();

                    if (suppliers.Count == 0)
                    {
                        Console.WriteLine("Please add a supplier first.");
                        break;
                    }

                    Console.WriteLine("\nAvailable Suppliers:");

                    foreach (var s in suppliers)
                    {
                        Console.WriteLine(
                            $"{s.SupplierId} | {s.SupplierName}");
                    }

                    Console.Write("Medicine code: ");
                    string code = Console.ReadLine() ?? "";

                    Console.Write("Medicine name: ");
                    string name = Console.ReadLine() ?? "";

                    Console.Write("Category: ");
                    string category = Console.ReadLine() ?? "";

                    Console.Write("Supplier ID: ");

                    if (!int.TryParse(Console.ReadLine(), out int supplierId))
                    {
                        Console.WriteLine("Invalid supplier ID.");
                        break;
                    }

                    if (!suppliers.Exists(s => s.SupplierId == supplierId))
                    {
                        Console.WriteLine("Supplier ID not found.");
                        break;
                    }

                    Console.Write("Batch number: ");
                    string batchNumber = Console.ReadLine() ?? "";

                    Console.Write("Expiry date (yyyy-MM-dd): ");

                    if (!DateTime.TryParse(
                        Console.ReadLine(), out DateTime expiryDate))
                    {
                        Console.WriteLine("Invalid expiry date.");
                        break;
                    }

                    Console.Write("Unit price: ");

                    if (!decimal.TryParse(
                        Console.ReadLine(), out decimal price))
                    {
                        Console.WriteLine("Invalid unit price.");
                        break;
                    }

                    Console.Write("Stock quantity: ");

                    if (!int.TryParse(
                        Console.ReadLine(), out int stock))
                    {
                        Console.WriteLine("Invalid stock quantity.");
                        break;
                    }

                    Console.Write("Reorder level: ");

                    if (!int.TryParse(
                        Console.ReadLine(), out int reorderLevel))
                    {
                        Console.WriteLine("Invalid reorder level.");
                        break;
                    }

                    var medicine = new Medicine
                    {
                        MedicineCode = code,
                        MedicineName = name,
                        Category = category,
                        SupplierId = supplierId,
                        BatchNumber = batchNumber,
                        ExpiryDate = expiryDate,
                        UnitPrice = price,
                        StockQuantity = stock,
                        ReorderLevel = reorderLevel
                    };

                    medicineService.Add(medicine);

                    Console.WriteLine("Medicine added successfully.");
                    break;
                }

            
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Validation error: {ex.Message}");
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
