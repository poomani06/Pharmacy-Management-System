
using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;

namespace PharmacyInventoryManagement.Services;

public class SupplierService
{
    private readonly SupplierRepository _supplierRepository;

    public SupplierService(SupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<int> AddSupplierAsync(Supplier supplier)
    {
        if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            throw new ArgumentException("Supplier name is required.");

        if (!string.IsNullOrWhiteSpace(supplier.Email) &&
            !supplier.Email.Contains('@'))
        {
            throw new ArgumentException("Enter a valid supplier email.");
        }

        return await _supplierRepository.AddAsync(supplier);
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _supplierRepository.GetAllAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int supplierId)
    {
        if (supplierId <= 0)
            throw new ArgumentException("Supplier ID must be greater than zero.");

        return await _supplierRepository.GetByIdAsync(supplierId);
    }

    public async Task<List<Supplier>> SearchSuppliersAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            throw new ArgumentException("Enter a supplier name to search.");

        return await _supplierRepository.SearchAsync(searchText);
    }

    public async Task<bool> UpdateSupplierAsync(Supplier supplier)
    {
        if (supplier.SupplierId <= 0)
            throw new ArgumentException("Valid supplier ID is required.");

        if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            throw new ArgumentException("Supplier name is required.");

        if (!string.IsNullOrWhiteSpace(supplier.Email) &&
            !supplier.Email.Contains('@'))
        {
            throw new ArgumentException("Enter a valid supplier email.");
        }

        return await _supplierRepository.UpdateAsync(supplier);
    }


}
