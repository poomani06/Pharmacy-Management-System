using Microsoft.Data.SqlClient;

namespace PharmacyInventoryManagement.Helpers;

public static class DatabaseHelper
{
    private const string ConnectionString =
        @"Server=.\SQLEXPRESS;Database=PharmacyInventoryDB;" +
        @"Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}