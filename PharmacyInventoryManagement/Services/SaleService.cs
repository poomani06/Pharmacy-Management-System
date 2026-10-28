
using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;

namespace PharmacyInventoryManagement.Services;

public class SaleService
{
    private readonly SaleRepository _saleRepository;
    private readonly MedicineRepository _medicineRepository;

    public SaleService(
        SaleRepository saleRepository,
        MedicineRepository medicineRepository)
    {
        _saleRepository = saleRepository;
        _medicineRepository = medicineRepository;
    }

    public async Task<int> CreateSaleAsync(int medicineId, int quantity)
    {
        if (medicineId <= 0)
            throw new ArgumentException("Valid medicine ID is required.");

        if (quantity <= 0)
            throw new ArgumentException("Sale quantity must be greater than zero.");

        Medicine? medicine =
            await _medicineRepository.GetByIdAsync(medicineId);

        if (medicine == null || !medicine.IsActive)
            throw new InvalidOperationException("Medicine was not found or is inactive.");

        if (medicine.ExpiryDate.Date < DateTime.Today)
            throw new InvalidOperationException("Expired medicine cannot be sold.");

        if (medicine.StockQuantity < quantity)
            throw new InvalidOperationException("Insufficient medicine stock.");

        Sale sale = new Sale
        {
            MedicineId = medicine.MedicineId,
            MedicineNameSnapshot = medicine.MedicineName,
            Quantity = quantity,
            UnitPrice = medicine.UnitPrice,
            TotalAmount = medicine.UnitPrice * quantity,
            SoldAt = DateTime.Now,
            SaleStatus = "Completed"
        };

        return await _saleRepository.AddAsync(sale);
    }

}
