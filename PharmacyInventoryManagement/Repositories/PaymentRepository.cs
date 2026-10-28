
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using PharmacyInventoryManagement.Helpers;
using PharmacyInventoryManagement.Models;

namespace PharmacyInventoryManagement.Repositories
{
    public class PaymentRepository
    {
        public async Task<int> AddAsync(Payment payment)
        {
            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
                INSERT INTO Payments
                (
                    SaleId,
                    Amount,
                    PaymentMethod,
                    PaymentStatus,
                    TransactionReference,
                    PaidAt
                )
                VALUES
                (
                    @SaleId,
                    @Amount,
                    @PaymentMethod,
                    @PaymentStatus,
                    @TransactionReference,
                    @PaidAt
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.Add("@SaleId", SqlDbType.Int)
                .Value = payment.SaleId;

            SqlParameter amountParameter =
                command.Parameters.Add("@Amount", SqlDbType.Decimal);

            amountParameter.Precision = 18;
            amountParameter.Scale = 2;
            amountParameter.Value = payment.Amount;

            command.Parameters.Add(
                "@PaymentMethod", SqlDbType.NVarChar, 50)
                .Value = payment.PaymentMethod;

            command.Parameters.Add(
                "@PaymentStatus", SqlDbType.NVarChar, 50)
                .Value = payment.PaymentStatus;

            command.Parameters.Add(
                "@TransactionReference", SqlDbType.NVarChar, 100)
                .Value = (object?)payment.TransactionReference
                         ?? DBNull.Value;

            command.Parameters.Add("@PaidAt", SqlDbType.DateTime)
                .Value = payment.PaidAt;

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }
    }
}
