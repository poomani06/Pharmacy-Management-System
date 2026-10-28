using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;

namespace PharmacyInventoryManagement.Services;

public class MedicineService
{
    private readonly MedicineRepository _repository;

    public MedicineService(MedicineRepository repository)
    {
        _repository = repository;
    }

    public int Add(Medicine medicine)
    {

        if (string.IsNullOrWhiteSpace(medicine.MedicineCode))
            throw new ArgumentException("Medicine code is required.");

        if (string.IsNullOrWhiteSpace(medicine.MedicineName))
            throw new ArgumentException("Medicine name is required.");

        if (string.IsNullOrWhiteSpace(medicine.BatchNumber))
            throw new ArgumentException("Batch number is required.");

        if (medicine.SupplierId <= 0)
            throw new ArgumentException("Enter a valid supplier ID.");

        if (medicine.ExpiryDate.Date < DateTime.Today)
            throw new ArgumentException("Expiry date cannot be in the past.");

        if (medicine.UnitPrice < 0)
            throw new ArgumentException("Price cannot be negative.");

        if (medicine.StockQuantity < 0 || medicine.ReorderLevel < 0)
            throw new ArgumentException("Stock and reorder level cannot be negative.");

        return _repository.Add(medicine);
    }

    
    
}