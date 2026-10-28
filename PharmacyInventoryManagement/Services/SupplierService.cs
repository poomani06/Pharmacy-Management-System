
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
}
