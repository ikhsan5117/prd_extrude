
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

class Program
{
    static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Checking database: {connectionString}");

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connected successfully!");

                CheckTable(connection, "ProductionReports");
                CheckTable(connection, "DimensionReports");
                CheckTable(connection, "MasterlistSpsDoubleLayers");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void CheckTable(SqlConnection conn, string tableName)
    {
        Console.WriteLine($"\nColumns in table '{tableName}':");
        using (var command = new SqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'", conn))
        using (var reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
            {
                Console.WriteLine("  [Table not found or no columns]");
                return;
            }
            while (reader.Read())
            {
                Console.WriteLine($"  - {reader.GetString(0)}");
            }
        }
    }
}
