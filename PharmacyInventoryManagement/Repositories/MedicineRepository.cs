using Microsoft.Data.SqlClient;
using PharmacyInventoryManagement.Helpers;
using PharmacyInventoryManagement.Models;

namespace PharmacyInventoryManagement.Repositories;

public class MedicineRepository
{
    public int Add(Medicine medicine)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        string query = @"
            INSERT INTO Medicines
            (MedicineCode, MedicineName, Category, SupplierId,
             BatchNumber, ExpiryDate, UnitPrice, StockQuantity,
             ReorderLevel, IsActive)
            VALUES
            (@Code, @Name, @Category, @SupplierId,
             @Batch, @Expiry, @Price, @Stock, @Reorder, 1);

            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Code", medicine.MedicineCode);
        command.Parameters.AddWithValue("@Name", medicine.MedicineName);
        command.Parameters.AddWithValue("@Category",
            (object?)medicine.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("@SupplierId", medicine.SupplierId);
        command.Parameters.AddWithValue("@Batch", medicine.BatchNumber);
        command.Parameters.AddWithValue("@Expiry", medicine.ExpiryDate.Date);
        command.Parameters.AddWithValue("@Price", medicine.UnitPrice);
        command.Parameters.AddWithValue("@Stock", medicine.StockQuantity);
        command.Parameters.AddWithValue("@Reorder", medicine.ReorderLevel);

        return (int)command.ExecuteScalar()!;
    }

    
    
}