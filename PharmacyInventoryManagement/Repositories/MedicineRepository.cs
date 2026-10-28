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

    public List<Medicine> GetAll()
    {
        List<Medicine> medicines = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        string query = @"
            SELECT MedicineId, MedicineCode, MedicineName, Category,
                   SupplierId, BatchNumber, ExpiryDate, UnitPrice,
                   StockQuantity, ReorderLevel, IsActive, CreatedAt
            FROM Medicines
            ORDER BY MedicineName;";

        using SqlCommand command = new(query, connection);
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            medicines.Add(MapMedicine(reader));
        }

        return medicines;
    }

    public List<Medicine> Search(string searchText)
    {
        List<Medicine> medicines = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        string query = @"
            SELECT MedicineId, MedicineCode, MedicineName, Category,
                   SupplierId, BatchNumber, ExpiryDate, UnitPrice,
                   StockQuantity, ReorderLevel, IsActive, CreatedAt
            FROM Medicines
            WHERE MedicineName LIKE @Search
               OR MedicineCode LIKE @Search
            ORDER BY MedicineName;";

        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Search", "%" + searchText + "%");

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            medicines.Add(MapMedicine(reader));
        }

        return medicines;
    }

    public List<Medicine> GetLowStock()
    {
        List<Medicine> medicines = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        string query = @"
            SELECT MedicineId, MedicineCode, MedicineName, Category,
                   SupplierId, BatchNumber, ExpiryDate, UnitPrice,
                   StockQuantity, ReorderLevel, IsActive, CreatedAt
            FROM Medicines
            WHERE IsActive = 1
              AND StockQuantity <= ReorderLevel
            ORDER BY StockQuantity;";

        using SqlCommand command = new(query, connection);
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            medicines.Add(MapMedicine(reader));
        }

        return medicines;
    }

    public List<Medicine> GetExpiring(int days)
    {
        List<Medicine> medicines = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        string query = @"
            SELECT MedicineId, MedicineCode, MedicineName, Category,
                   SupplierId, BatchNumber, ExpiryDate, UnitPrice,
                   StockQuantity, ReorderLevel, IsActive, CreatedAt
            FROM Medicines
            WHERE IsActive = 1
              AND StockQuantity > 0
              AND ExpiryDate >= CAST(GETDATE() AS DATE)
              AND ExpiryDate <= DATEADD(DAY, @Days, CAST(GETDATE() AS DATE))
            ORDER BY ExpiryDate;";

        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Days", days);

        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            medicines.Add(MapMedicine(reader));
        }

        return medicines;
    }

    public List<Medicine> GetExpired()
    {
        List<Medicine> medicines = new();

        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        string query = @"
            SELECT MedicineId, MedicineCode, MedicineName, Category,
                   SupplierId, BatchNumber, ExpiryDate, UnitPrice,
                   StockQuantity, ReorderLevel, IsActive, CreatedAt
            FROM Medicines
            WHERE ExpiryDate < CAST(GETDATE() AS DATE)
              AND StockQuantity > 0
            ORDER BY ExpiryDate;";

        using SqlCommand command = new(query, connection);
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            medicines.Add(MapMedicine(reader));
        }

        return medicines;
    }

    public void UpdatePriceAndStock(
        int medicineId,
        decimal newPrice,
        int newStock)
    {
        using SqlConnection connection = DatabaseHelper.CreateConnection();
        connection.Open();

        string query = @"
            UPDATE Medicines
            SET UnitPrice = @Price,
                StockQuantity = @Stock
            WHERE MedicineId = @Id AND IsActive = 1;";

        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Price", newPrice);
        command.Parameters.AddWithValue("@Stock", newStock);
        command.Parameters.AddWithValue("@Id", medicineId);

        int rows = command.ExecuteNonQuery();

        if (rows == 0)
        {
            throw new InvalidOperationException(
                "Medicine not found or inactive.");
        }
    }

    private static Medicine MapMedicine(SqlDataReader reader)
    {
        return new Medicine
        {
            MedicineId = reader.GetInt32(reader.GetOrdinal("MedicineId")),
            MedicineCode = reader.GetString(reader.GetOrdinal("MedicineCode")),
            MedicineName = reader.GetString(reader.GetOrdinal("MedicineName")),
            Category = reader.IsDBNull(reader.GetOrdinal("Category"))
                ? null
                : reader.GetString(reader.GetOrdinal("Category")),
            SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),
            BatchNumber = reader.GetString(reader.GetOrdinal("BatchNumber")),
            ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
            UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
            StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
            ReorderLevel = reader.GetInt32(reader.GetOrdinal("ReorderLevel")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}