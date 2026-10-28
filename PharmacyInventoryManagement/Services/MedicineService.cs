
using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;

namespace PharmacyInventoryManagement.Services;

public class MedicineService
{
    private readonly MedicineRepository _medicineRepository;

    public MedicineService(MedicineRepository medicineRepository)
    {
        _medicineRepository = medicineRepository;
    }

    public async Task<int> AddMedicineAsync(Medicine medicine)
    {
        if (string.IsNullOrWhiteSpace(medicine.MedicineCode))
            throw new ArgumentException("Medicine code is required.");

        if (string.IsNullOrWhiteSpace(medicine.MedicineName))
            throw new ArgumentException("Medicine name is required.");

        if (medicine.SupplierId <= 0)
            throw new ArgumentException("A valid supplier is required.");

        if (medicine.UnitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.");

        if (medicine.StockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.");

        if (medicine.ReorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative.");

        if (medicine.ExpiryDate.Date < DateTime.Today)
            throw new ArgumentException("Expiry date cannot be in the past.");

        return await _medicineRepository.AddAsync(medicine);
    }

    public async Task<List<Medicine>> GetAllMedicinesAsync()
    {
        return await _medicineRepository.GetAllAsync();
    }

    public async Task<Medicine?> GetMedicineByIdAsync(int medicineId)
    {
        if (medicineId <= 0)
            throw new ArgumentException("Medicine ID must be greater than zero.");

        return await _medicineRepository.GetByIdAsync(medicineId);
    }

}
