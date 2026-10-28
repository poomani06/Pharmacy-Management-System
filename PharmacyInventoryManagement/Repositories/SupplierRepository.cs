
using Microsoft.Data.SqlClient;
using System.Data;
using PharmacyInventoryManagement.Helpers;
using PharmacyInventoryManagement.Models;

namespace PharmacyInventoryManagement.Repositories;

public class SupplierRepository
{
    public async Task<int> AddAsync(Supplier supplier)
    {
        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
            INSERT INTO Suppliers
            (
                SupplierName,
                Phone,
                Email,
                Address,
                IsActive
            )
            VALUES
            (
                @SupplierName,
                @Phone,
                @Email,
                @Address,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 100)
            .Value = supplier.SupplierName;

        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20)
            .Value = (object?)supplier.Phone ?? DBNull.Value;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100)
            .Value = (object?)supplier.Email ?? DBNull.Value;

        command.Parameters.Add("@Address", SqlDbType.NVarChar, 250)
            .Value = (object?)supplier.Address ?? DBNull.Value;

        command.Parameters.Add("@IsActive", SqlDbType.Bit)
            .Value = true;

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        List<Supplier> suppliers = new List<Supplier>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        SELECT
            SupplierId,
            SupplierName,
            Phone,
            Email,
            Address,
            IsActive
        FROM Suppliers
        ORDER BY SupplierId;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Supplier supplier = new Supplier
            {
                SupplierId = reader.GetInt32(0),
                SupplierName = reader.GetString(1),
                Phone = reader.IsDBNull(2) ? null : reader.GetString(2),
                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            };

            suppliers.Add(supplier);
        }

        return suppliers;
    }

    public async Task<Supplier?> GetByIdAsync(int supplierId)
    {
        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        SELECT
            SupplierId,
            SupplierName,
            Phone,
            Email,
            Address,
            IsActive
        FROM Suppliers
        WHERE SupplierId = @SupplierId;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@SupplierId", SqlDbType.Int)
            .Value = supplierId;

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            Supplier supplier = new Supplier
            {
                SupplierId = reader.GetInt32(0),
                SupplierName = reader.GetString(1),
                Phone = reader.IsDBNull(2) ? null : reader.GetString(2),
                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            };

            return supplier;
        }

        return null;
    }

    public async Task<List<Supplier>> SearchAsync(string searchText)
    {
        List<Supplier> suppliers = new List<Supplier>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        SELECT
            SupplierId,
            SupplierName,
            Phone,
            Email,
            Address,
            IsActive
        FROM Suppliers
        WHERE SupplierName LIKE @SearchText
        ORDER BY SupplierName;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 100)
            .Value = "%" + searchText.Trim() + "%";

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Supplier supplier = new Supplier
            {
                SupplierId = reader.GetInt32(0),
                SupplierName = reader.GetString(1),
                Phone = reader.IsDBNull(2) ? null : reader.GetString(2),
                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            };

            suppliers.Add(supplier);
        }

        return suppliers;
    }

    public async Task<bool> UpdateAsync(Supplier supplier)
    {
        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        UPDATE Suppliers
        SET
            SupplierName = @SupplierName,
            Phone = @Phone,
            Email = @Email,
            Address = @Address
        WHERE SupplierId = @SupplierId
          AND IsActive = 1;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@SupplierId", SqlDbType.Int)
            .Value = supplier.SupplierId;

        command.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 100)
            .Value = supplier.SupplierName;

        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20)
            .Value = (object?)supplier.Phone ?? DBNull.Value;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100)
            .Value = (object?)supplier.Email ?? DBNull.Value;

        command.Parameters.Add("@Address", SqlDbType.NVarChar, 250)
            .Value = (object?)supplier.Address ?? DBNull.Value;

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }




}
