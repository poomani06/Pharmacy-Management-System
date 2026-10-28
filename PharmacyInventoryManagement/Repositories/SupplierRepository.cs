
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
}
