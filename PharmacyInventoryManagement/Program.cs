
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;
using PharmacyInventoryManagement.Services;

namespace PharmacyInventoryManagement
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var medicineRepository = new MedicineRepository();
            var supplierRepository = new SupplierRepository();
            var saleRepository = new SaleRepository();
            var paymentRepository = new PaymentRepository();

            var medicineService = new MedicineService(medicineRepository);
            var supplierService = new SupplierService(supplierRepository);
            var saleService = new SaleService(saleRepository, medicineRepository);
            var paymentService = new PaymentService(paymentRepository, saleRepository);

            while (true)
            {
                Console.WriteLine("\n=== PHARMACY INVENTORY MANAGEMENT ===");
                Console.WriteLine("1. Add Medicine");
                Console.WriteLine("2. View All Medicines");
                Console.WriteLine("3. View Low-Stock Medicines");
                Console.WriteLine("4. View Expired Medicines");
                Console.WriteLine("5. View Expiring Medicines");
                Console.WriteLine("6. Add Supplier");
                Console.WriteLine("7. View All Suppliers");
                Console.WriteLine("8. Create Sale");
                Console.WriteLine("9. View All Sales");
                Console.WriteLine("10. Add Payment");
                Console.WriteLine("11. View All Payments");
                Console.WriteLine("12. View Payments by Sale ID");
                Console.WriteLine("13. Update Payment Status");
                Console.WriteLine("14. Mark Payment as Failed");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            {
                                var medicine = new Medicine();

                                Console.Write("Medicine code: ");
                                medicine.MedicineCode = Console.ReadLine() ?? "";

                                Console.Write("Medicine name: ");
                                medicine.MedicineName = Console.ReadLine() ?? "";

                                Console.Write("Category: ");
                                medicine.Category = Console.ReadLine();

                                Console.Write("Supplier ID: ");
                                int.TryParse(Console.ReadLine(), out int supplierId);
                                medicine.SupplierId = supplierId;

                                Console.Write("Batch number: ");
                                medicine.BatchNumber = Console.ReadLine() ?? "";

                                Console.Write("Expiry date (yyyy-MM-dd): ");
                                DateTime.TryParse(Console.ReadLine(), out DateTime expiryDate);
                                medicine.ExpiryDate = expiryDate;

                                Console.Write("Unit price: ");
                                decimal.TryParse(Console.ReadLine(), out decimal price);
                                medicine.UnitPrice = price;

                                Console.Write("Stock quantity: ");
                                int.TryParse(Console.ReadLine(), out int stock);
                                medicine.StockQuantity = stock;

                                Console.Write("Reorder level: ");
                                int.TryParse(Console.ReadLine(), out int reorder);
                                medicine.ReorderLevel = reorder;

                                medicine.IsActive = true;
                                medicine.CreatedAt = DateTime.Now;

                                await medicineService.AddMedicineAsync(medicine);

                                Console.WriteLine("Medicine added successfully.");
                                break;
                            }

                        case "2":
                            {
                                var medicines = await medicineService.GetAllMedicinesAsync();

                                foreach (var m in medicines)
                                {
                                    Console.WriteLine(
                                        $"{m.MedicineId} | {m.MedicineCode} | " +
                                        $"{m.MedicineName} | {m.StockQuantity} | {m.UnitPrice}");
                                }

                                break;
                            }

                        case "3":
                            {
                                var medicines = await medicineService.GetLowStockMedicinesAsync();

                                foreach (var m in medicines)
                                {
                                    Console.WriteLine(
                                        $"{m.MedicineName} | Stock: {m.StockQuantity} | " +
                                        $"Reorder level: {m.ReorderLevel}");
                                }

                                break;
                            }

                        case "4":
                            {
                                var medicines = await medicineService.GetExpiredMedicinesAsync();

                                foreach (var m in medicines)
                                {
                                    Console.WriteLine(
                                        $"{m.MedicineId} | {m.MedicineName} | {m.ExpiryDate:yyyy-MM-dd}");
                                }

                                break;
                            }

                        case "5":
                            {
                                Console.Write("Enter number of days: ");
                                int.TryParse(Console.ReadLine(), out int days);

                                var medicines = await medicineService.GetExpiringMedicinesAsync(days);

                                foreach (var m in medicines)
                                {
                                    Console.WriteLine(
                                        $"{m.MedicineName} | {m.ExpiryDate:yyyy-MM-dd}");
                                }

                                break;
                            }

                        case "6":
                            {
                                var supplier = new Supplier();

                                Console.Write("Supplier name: ");
                                supplier.SupplierName = Console.ReadLine() ?? "";

                                Console.Write("Phone: ");
                                supplier.Phone = Console.ReadLine();

                                Console.Write("Email: ");
                                supplier.Email = Console.ReadLine();

                                Console.Write("Address: ");
                                supplier.Address = Console.ReadLine();

                                supplier.IsActive = true;

                                await supplierService.AddSupplierAsync(supplier);

                                Console.WriteLine("Supplier added successfully.");
                                break;
                            }

                        case "7":
                            {
                                var suppliers = await supplierService.GetAllSuppliersAsync();

                                foreach (var s in suppliers)
                                {
                                    Console.WriteLine(
                                        $"{s.SupplierId} | {s.SupplierName} | {s.Phone} | {s.Email}");
                                }

                                break;
                            }

                        case "8":
                            {
                                Console.Write("Medicine ID: ");
                                int.TryParse(Console.ReadLine(), out int medicineId);

                                Console.Write("Quantity: ");
                                int.TryParse(Console.ReadLine(), out int quantity);

                                int saleId = await saleService.CreateSaleAsync(
                                    medicineId, quantity);

                                Console.WriteLine($"Sale created. Sale ID: {saleId}");
                                break;
                            }

                        case "9":
                            {
                                var sales = await saleService.GetAllSalesAsync();

                                foreach (var s in sales)
                                {
                                    Console.WriteLine(
                                        $"{s.SaleId} | {s.MedicineNameSnapshot} | " +
                                        $"{s.Quantity} | {s.TotalAmount} | {s.SaleStatus}");
                                }

                                break;
                            }

                        case "10":
                            {
                                var payment = new Payment();

                                Console.Write("Sale ID: ");
                                int.TryParse(Console.ReadLine(), out int saleId);
                                payment.SaleId = saleId;

                                Console.Write("Payment amount: ");
                                decimal.TryParse(Console.ReadLine(), out decimal amount);
                                payment.Amount = amount;

                                Console.Write("Payment method: ");
                                payment.PaymentMethod = Console.ReadLine() ?? "Cash";

                                Console.Write("Transaction reference: ");
                                payment.TransactionReference = Console.ReadLine();

                                int paymentId = await paymentService.AddPaymentAsync(payment);

                                Console.WriteLine($"Payment added. Payment ID: {paymentId}");
                                break;
                            }

                        case "11":
                            {
                                var payments = await paymentService.GetAllPaymentsAsync();

                                foreach (var p in payments)
                                {
                                    Console.WriteLine(
                                        $"{p.PaymentId} | Sale ID: {p.SaleId} | " +
                                        $"{p.Amount} | {p.PaymentMethod} | {p.PaymentStatus}");
                                }

                                break;
                            }

                        case "12":
                            {
                                Console.Write("Sale ID: ");
                                int.TryParse(Console.ReadLine(), out int saleId);

                                var payments = await paymentService.GetPaymentsBySaleIdAsync(saleId);

                                foreach (var p in payments)
                                {
                                    Console.WriteLine(
                                        $"{p.PaymentId} | {p.Amount} | " +
                                        $"{p.PaymentMethod} | {p.PaymentStatus}");
                                }

                                break;
                            }

                        case "13":
                            {
                                Console.Write("Payment ID: ");
                                int.TryParse(Console.ReadLine(), out int paymentId);

                                Console.Write("New status (Pending/Paid/Failed/Refunded): ");
                                string status = Console.ReadLine() ?? "";

                                bool updated = await paymentService.UpdatePaymentStatusAsync(
                                    paymentId, status);

                                Console.WriteLine(updated
                                    ? "Payment status updated."
                                    : "Payment not found.");

                                break;
                            }

                        case "14":
                            {
                                Console.Write("Payment ID: ");
                                int.TryParse(Console.ReadLine(), out int paymentId);

                                bool updated = await paymentService.MarkPaymentAsFailedAsync(
                                    paymentId);

                                Console.WriteLine(updated
                                    ? "Payment marked as failed."
                                    : "Payment not found.");

                                break;
                            }

                        case "0":
                            Console.WriteLine("Application closed.");
                            return;

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

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }
    }
}
