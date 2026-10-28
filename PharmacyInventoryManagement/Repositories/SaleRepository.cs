
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

}
