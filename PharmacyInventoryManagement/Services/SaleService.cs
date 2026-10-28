
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

    public async Task<List<Sale>> GetAllSalesAsync()
    {
        return await _saleRepository.GetAllAsync();
    }

    public async Task<Sale?> GetSaleByIdAsync(int saleId)
    {
        if (saleId <= 0)
        {
            throw new ArgumentException("Sale ID must be greater than zero.");
        }

        return await _saleRepository.GetByIdAsync(saleId);
    }

    public async Task<List<Sale>> SearchSalesAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            throw new ArgumentException("Medicine name cannot be empty.");
        }

        return await _saleRepository.SearchAsync(searchText);
    }

    public async Task<List<Sale>> GetSalesByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate.Date > endDate.Date)
        {
            throw new ArgumentException(
                "Start date cannot be after the end date.");
        }

        return await _saleRepository.GetByDateRangeAsync(
            startDate,
            endDate);
    }

    public async Task<decimal> GetTotalSalesAsync()
    {
        return await _saleRepository.GetTotalSalesAsync();
    }

    public async Task<decimal> GetTodaySalesAsync()
    {
        return await _saleRepository.GetTodaySalesAsync();
    }

    public async Task<int> GetTotalQuantitySoldAsync()
    {
        return await _saleRepository.GetTotalQuantitySoldAsync();
    }


    public async Task<bool> CancelSaleAsync(int saleId)
    {
        if (saleId <= 0)
        {
            throw new ArgumentException(
                "Sale ID must be greater than zero.");
        }

        return await _saleRepository.CancelAsync(saleId);
    }


}
