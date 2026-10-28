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

    public async Task<Medicine?> GetByIdAsync(int medicineId)
    {
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
        WHERE MedicineId = @MedicineId;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@MedicineId", SqlDbType.Int)
            .Value = medicineId;

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
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

            return medicine;
        }

        return null;
    }

    public async Task<List<Medicine>> SearchAsync(string searchText)
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
        WHERE MedicineName LIKE @SearchText
           OR MedicineCode LIKE @SearchText
        ORDER BY MedicineName;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 100)
            .Value = "%" + searchText.Trim() + "%";

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

    public async Task<bool> UpdateAsync(Medicine medicine)
    {
        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        UPDATE Medicines
        SET
            MedicineCode = @MedicineCode,
            MedicineName = @MedicineName,
            Category = @Category,
            SupplierId = @SupplierId,
            BatchNumber = @BatchNumber,
            ExpiryDate = @ExpiryDate,
            UnitPrice = @UnitPrice,
            StockQuantity = @StockQuantity,
            ReorderLevel = @ReorderLevel
        WHERE MedicineId = @MedicineId
          AND IsActive = 1;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@MedicineId", SqlDbType.Int)
            .Value = medicine.MedicineId;

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

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int medicineId)
    {
        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        UPDATE Medicines
        SET IsActive = 0
        WHERE MedicineId = @MedicineId
          AND IsActive = 1;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@MedicineId", SqlDbType.Int)
            .Value = medicineId;

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<List<Medicine>> GetLowStockAsync()
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
        WHERE StockQuantity <= ReorderLevel
          AND IsActive = 1
        ORDER BY StockQuantity ASC;
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

    public async Task<List<Medicine>> GetExpiringAsync(int days)
    {
        if (days < 1 || days > 365)
            throw new ArgumentException("Days must be between 1 and 365.");

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
        WHERE ExpiryDate >= CAST(GETDATE() AS DATE)
          AND ExpiryDate <= DATEADD(DAY, @Days, CAST(GETDATE() AS DATE))
          AND IsActive = 1
        ORDER BY ExpiryDate ASC;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@Days", SqlDbType.Int).Value = days;

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

    public async Task<List<Medicine>> GetExpiredAsync()
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
        WHERE ExpiryDate < CAST(GETDATE() AS DATE)
          AND IsActive = 1
        ORDER BY ExpiryDate ASC;
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