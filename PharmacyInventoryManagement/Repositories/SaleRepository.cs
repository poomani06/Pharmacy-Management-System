
using Microsoft.Data.SqlClient;
using System.Data;
using PharmacyInventoryManagement.Helpers;
using PharmacyInventoryManagement.Models;

namespace PharmacyInventoryManagement.Repositories;

public class SaleRepository
{
    public async Task<int> AddAsync(Sale sale)
    {
        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        using SqlTransaction transaction =
            (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            string stockSql = """
                UPDATE Medicines
                SET StockQuantity = StockQuantity - @Quantity
                WHERE MedicineId = @MedicineId
                  AND IsActive = 1
                  AND ExpiryDate >= CAST(GETDATE() AS DATE)
                  AND StockQuantity >= @Quantity;
                """;

            await using SqlCommand stockCommand =
                new SqlCommand(stockSql, connection, transaction);

            stockCommand.Parameters.Add("@MedicineId", SqlDbType.Int)
                .Value = sale.MedicineId;

            stockCommand.Parameters.Add("@Quantity", SqlDbType.Int)
                .Value = sale.Quantity;

            int stockUpdated =
                await stockCommand.ExecuteNonQueryAsync();

            if (stockUpdated == 0)
            {
                throw new InvalidOperationException(
                    "Medicine is unavailable, expired, or has insufficient stock.");
            }

            string saleSql = """
                INSERT INTO Sales
                (
                    MedicineId,
                    MedicineNameSnapshot,
                    Quantity,
                    UnitPrice,
                    TotalAmount,
                    SoldAt,
                    SaleStatus
                )
                VALUES
                (
                    @MedicineId,
                    @MedicineNameSnapshot,
                    @Quantity,
                    @UnitPrice,
                    @TotalAmount,
                    @SoldAt,
                    @SaleStatus
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            await using SqlCommand saleCommand =
                new SqlCommand(saleSql, connection, transaction);

            saleCommand.Parameters.Add("@MedicineId", SqlDbType.Int)
                .Value = sale.MedicineId;

            saleCommand.Parameters.Add(
                "@MedicineNameSnapshot", SqlDbType.NVarChar, 100)
                .Value = sale.MedicineNameSnapshot;

            saleCommand.Parameters.Add("@Quantity", SqlDbType.Int)
                .Value = sale.Quantity;

            SqlParameter unitPriceParameter =
                saleCommand.Parameters.Add("@UnitPrice", SqlDbType.Decimal);

            unitPriceParameter.Precision = 10;
            unitPriceParameter.Scale = 2;
            unitPriceParameter.Value = sale.UnitPrice;

            SqlParameter totalAmountParameter =
                saleCommand.Parameters.Add("@TotalAmount", SqlDbType.Decimal);

            totalAmountParameter.Precision = 10;
            totalAmountParameter.Scale = 2;
            totalAmountParameter.Value = sale.TotalAmount;

            saleCommand.Parameters.Add("@SoldAt", SqlDbType.DateTime2)
                .Value = sale.SoldAt;

            saleCommand.Parameters.Add("@SaleStatus", SqlDbType.NVarChar, 20)
                .Value = sale.SaleStatus;

            object? result = await saleCommand.ExecuteScalarAsync();

            int saleId = Convert.ToInt32(result);

            await transaction.CommitAsync();

            return saleId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        List<Sale> sales = new List<Sale>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await connection.OpenAsync();

        string sql = """
        SELECT
            SaleId,
            MedicineId,
            MedicineNameSnapshot,
            Quantity,
            UnitPrice,
            TotalAmount,
            SoldAt,
            SaleStatus
        FROM Sales
        ORDER BY SoldAt DESC;
        """;

        await using SqlCommand command =
            new SqlCommand(sql, connection);

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Sale sale = new Sale
            {
                SaleId = reader.GetInt32(0),
                MedicineId = reader.GetInt32(1),
                MedicineNameSnapshot = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                UnitPrice = reader.GetDecimal(4),
                TotalAmount = reader.GetDecimal(5),
                SoldAt = reader.GetDateTime(6),
                SaleStatus = reader.GetString(7)
            };

            sales.Add(sale);
        }

        return sales;
    }

    public async Task<Sale?> GetByIdAsync(int saleId)
    {
        await using var connection = DatabaseHelper.CreateConnection();
        await connection.OpenAsync();

        const string query = """
        SELECT SaleId, MedicineId, MedicineNameSnapshot,
               Quantity, UnitPrice, TotalAmount, SoldAt, SaleStatus
        FROM Sales
        WHERE SaleId = @SaleId;
        """;

        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@SaleId", SqlDbType.Int).Value = saleId;

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Sale
            {
                SaleId = reader.GetInt32(0),
                MedicineId = reader.GetInt32(1),
                MedicineNameSnapshot = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                UnitPrice = reader.GetDecimal(4),
                TotalAmount = reader.GetDecimal(5),
                SoldAt = reader.GetDateTime(6),
                SaleStatus = reader.GetString(7)
            };
        }

        return null;
    }


    public async Task<List<Sale>> SearchAsync(string searchText)
    {
        List<Sale> sales = new List<Sale>();

        await using var connection = DatabaseHelper.CreateConnection();
        await connection.OpenAsync();

        const string query = """
        SELECT SaleId, MedicineId, MedicineNameSnapshot,
               Quantity, UnitPrice, TotalAmount, SoldAt, SaleStatus
        FROM Sales
        WHERE MedicineNameSnapshot LIKE @SearchText
        ORDER BY SoldAt DESC;
        """;

        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 200)
            .Value = "%" + searchText.Trim() + "%";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Sale sale = new Sale
            {
                SaleId = reader.GetInt32(0),
                MedicineId = reader.GetInt32(1),
                MedicineNameSnapshot = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                UnitPrice = reader.GetDecimal(4),
                TotalAmount = reader.GetDecimal(5),
                SoldAt = reader.GetDateTime(6),
                SaleStatus = reader.GetString(7)
            };

            sales.Add(sale);
        }

        return sales;
    }

    public async Task<List<Sale>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        List<Sale> sales = new List<Sale>();

        await using var connection = DatabaseHelper.CreateConnection();
        await connection.OpenAsync();

        const string query = """
        SELECT SaleId, MedicineId, MedicineNameSnapshot,
               Quantity, UnitPrice, TotalAmount, SoldAt, SaleStatus
        FROM Sales
        WHERE SoldAt >= @StartDate
          AND SoldAt < @EndDate
        ORDER BY SoldAt DESC;
        """;

        await using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value =
            startDate.Date;

        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value =
            endDate.Date.AddDays(1);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Sale sale = new Sale
            {
                SaleId = reader.GetInt32(0),
                MedicineId = reader.GetInt32(1),
                MedicineNameSnapshot = reader.GetString(2),
                Quantity = reader.GetInt32(3),
                UnitPrice = reader.GetDecimal(4),
                TotalAmount = reader.GetDecimal(5),
                SoldAt = reader.GetDateTime(6),
                SaleStatus = reader.GetString(7)
            };

            sales.Add(sale);
        }

        return sales;
    }


}
