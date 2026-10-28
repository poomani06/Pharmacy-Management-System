
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

        public async Task<List<Payment>> GetAllAsync()
        {
            List<Payment> payments = new List<Payment>();

            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
        SELECT PaymentId, SaleId, Amount, PaymentMethod,
               PaymentStatus, TransactionReference, PaidAt
        FROM Payments
        ORDER BY PaidAt DESC;
        """;

            await using var command = new SqlCommand(query, connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Payment payment = new Payment
                {
                    PaymentId = reader.GetInt32(0),
                    SaleId = reader.GetInt32(1),
                    Amount = reader.GetDecimal(2),
                    PaymentMethod = reader.GetString(3),
                    PaymentStatus = reader.GetString(4),
                    TransactionReference = reader.IsDBNull(5)
                        ? null
                        : reader.GetString(5),
                    PaidAt = reader.GetDateTime(6)
                };

                payments.Add(payment);
            }

            return payments;
        }

        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
        SELECT PaymentId, SaleId, Amount, PaymentMethod,
               PaymentStatus, TransactionReference, PaidAt
        FROM Payments
        WHERE PaymentId = @PaymentId;
        """;

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@PaymentId", SqlDbType.Int)
                .Value = paymentId;

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Payment
                {
                    PaymentId = reader.GetInt32(0),
                    SaleId = reader.GetInt32(1),
                    Amount = reader.GetDecimal(2),
                    PaymentMethod = reader.GetString(3),
                    PaymentStatus = reader.GetString(4),
                    TransactionReference = reader.IsDBNull(5)
                        ? null
                        : reader.GetString(5),
                    PaidAt = reader.GetDateTime(6)
                };
            }

            return null;
        }

        public async Task<List<Payment>> SearchByMethodAsync(
            string paymentMethod)
        {
            List<Payment> payments = new List<Payment>();

            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
        SELECT PaymentId, SaleId, Amount, PaymentMethod,
               PaymentStatus, TransactionReference, PaidAt
        FROM Payments
        WHERE PaymentMethod = @PaymentMethod
        ORDER BY PaidAt DESC;
        """;

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add(
                "@PaymentMethod", SqlDbType.NVarChar, 50)
                .Value = paymentMethod.Trim();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Payment payment = new Payment
                {
                    PaymentId = reader.GetInt32(0),
                    SaleId = reader.GetInt32(1),
                    Amount = reader.GetDecimal(2),
                    PaymentMethod = reader.GetString(3),
                    PaymentStatus = reader.GetString(4),
                    TransactionReference = reader.IsDBNull(5)
                        ? null
                        : reader.GetString(5),
                    PaidAt = reader.GetDateTime(6)
                };

                payments.Add(payment);
            }

            return payments;
        }

        public async Task<decimal> GetTotalPaymentsAsync()
        {
            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
        SELECT ISNULL(SUM(Amount), 0)
        FROM Payments
        WHERE PaymentStatus = @PaymentStatus;
        """;

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add(
                "@PaymentStatus", SqlDbType.NVarChar, 50)
                .Value = "Paid";

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToDecimal(result);
        }

        public async Task<List<Payment>> GetPaymentsBySaleIdAsync(
            int saleId)
        {
            List<Payment> payments = new List<Payment>();

            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
        SELECT PaymentId, SaleId, Amount, PaymentMethod,
               PaymentStatus, TransactionReference, PaidAt
        FROM Payments
        WHERE SaleId = @SaleId
        ORDER BY PaidAt DESC;
        """;

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@SaleId", SqlDbType.Int)
                .Value = saleId;

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Payment payment = new Payment
                {
                    PaymentId = reader.GetInt32(0),
                    SaleId = reader.GetInt32(1),
                    Amount = reader.GetDecimal(2),
                    PaymentMethod = reader.GetString(3),
                    PaymentStatus = reader.GetString(4),
                    TransactionReference = reader.IsDBNull(5)
                        ? null
                        : reader.GetString(5),
                    PaidAt = reader.GetDateTime(6)
                };

                payments.Add(payment);
            }

            return payments;
        }

        public async Task<bool> UpdateStatusAsync(
            int paymentId,
            string paymentStatus)
        {
            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
        UPDATE Payments
        SET PaymentStatus = @PaymentStatus
        WHERE PaymentId = @PaymentId;
        """;

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@PaymentId", SqlDbType.Int)
                .Value = paymentId;

            command.Parameters.Add(
                "@PaymentStatus", SqlDbType.NVarChar, 50)
                .Value = paymentStatus;

            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<decimal> GetPaidAmountBySaleIdAsync(int saleId)
        {
            await using var connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            const string query = """
        SELECT ISNULL(SUM(Amount), 0)
        FROM Payments
        WHERE SaleId = @SaleId
          AND PaymentStatus = @PaymentStatus;
        """;

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@SaleId", SqlDbType.Int)
                .Value = saleId;

            command.Parameters.Add(
                "@PaymentStatus", SqlDbType.NVarChar, 50)
                .Value = "Paid";

            object? result = await command.ExecuteScalarAsync();

            return Convert.ToDecimal(result);
        }


    }

}
