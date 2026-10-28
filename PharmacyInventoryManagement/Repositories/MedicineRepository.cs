using System.Data;
using Microsoft.Data.SqlClient;
using PharmacyInventoryManagement.Helpers;
using PharmacyInventoryManagement.Models;

namespace PharmacyInventoryManagement.Repositories;

public class MedicineRepository
{
    public async Task<int> AddAsync(Medicine medicine)
    {
        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        INSERT INTO Medicines
        (
            MedicineCode,
            MedicineName,
            Category,
            SupplierId,
            BatchNumber,
            ExpiryDate,
            UnitPrice,
            StockQuantity,
            ReorderLevel,
            IsActive
        )
        VALUES
        (
            @MedicineCode,
            @MedicineName,
            @Category,
            @SupplierId,
            @BatchNumber,
            @ExpiryDate,
            @UnitPrice,
            @StockQuantity,
            @ReorderLevel,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@MedicineCode", SqlDbType.NVarChar, 50)
            .Value = medicine.MedicineCode;

        command.Parameters.Add("@MedicineName", SqlDbType.NVarChar, 100)
            .Value = medicine.MedicineName;

        command.Parameters.Add("@Category", SqlDbType.NVarChar, 50)
            .Value = (object?)medicine.Category ?? DBNull.Value;

        command.Parameters.Add("@SupplierId", SqlDbType.Int)
            .Value = medicine.SupplierId;

        command.Parameters.Add("@BatchNumber", SqlDbType.NVarChar, 50)
            .Value = medicine.BatchNumber;

        command.Parameters.Add("@ExpiryDate", SqlDbType.Date)
            .Value = medicine.ExpiryDate;

        SqlParameter priceParameter =
            command.Parameters.Add("@UnitPrice", SqlDbType.Decimal);

        priceParameter.Precision = 10;
        priceParameter.Scale = 2;
        priceParameter.Value = medicine.UnitPrice;

        command.Parameters.Add("@StockQuantity", SqlDbType.Int)
            .Value = medicine.StockQuantity;

        command.Parameters.Add("@ReorderLevel", SqlDbType.Int)
            .Value = medicine.ReorderLevel;

        command.Parameters.Add("@IsActive", SqlDbType.Bit)
            .Value = true;

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
    public async Task<List<Medicine>> GetAllAsync()
    {
        List<Medicine> medicines = new List<Medicine>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        SELECT
            MedicineId,
            MedicineCode,
            MedicineName,
            Category,
            SupplierId,
            BatchNumber,
            ExpiryDate,
            UnitPrice,
            StockQuantity,
            ReorderLevel,
            IsActive,
            CreatedAt
        FROM Medicines
        ORDER BY MedicineId;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Medicine medicine = new Medicine
            {
                MedicineId = reader.GetInt32(0),
                MedicineCode = reader.GetString(1),
                MedicineName = reader.GetString(2),
                Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                SupplierId = reader.GetInt32(4),
                BatchNumber = reader.GetString(5),
                ExpiryDate = reader.GetDateTime(6),
                UnitPrice = reader.GetDecimal(7),
                StockQuantity = reader.GetInt32(8),
                ReorderLevel = reader.GetInt32(9),
                IsActive = reader.GetBoolean(10),
                CreatedAt = reader.GetDateTime(11)
            };

            medicines.Add(medicine);
        }

        return medicines;
    }
}