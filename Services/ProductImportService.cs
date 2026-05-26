using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Npgsql;

namespace ProjectDTS;

public class ProductImportService
{
    private readonly string _connectionString;

    public ProductImportService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ImportProductsAsync(string filePath)
    {
        var config = new CsvConfiguration(
            CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null,
            Delimiter = ",",
            HasHeaderRecord = true
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        await using var conn =
            new NpgsqlConnection(_connectionString);

        await conn.OpenAsync();

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            try
            {
                var priceText = csv.GetField("Price");

                if (!decimal.TryParse(
                        priceText,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out decimal price))
                {
                    Console.WriteLine(
                        $"Skipped invalid price: {csv.GetField("Name")}"
                    );

                    continue;
                }

                var cmd = new NpgsqlCommand(@"
                INSERT INTO products
                (
                    name,
                    description,
                    category,
                    price,
                    rarity,
                    view_count,
                    purchase_count,
                    stock
                )
                VALUES
                (
                    @name,
                    @description,
                    @category,
                    @price,
                    @rarity,
                    @viewCount,
                    @purchaseCount,
                    @stock
                )

                ON CONFLICT(name)
                DO NOTHING
                ", conn);

                cmd.Parameters.AddWithValue(
                    "name",
                    csv.GetField("Name")
                );

                cmd.Parameters.AddWithValue(
                    "description",
                    csv.GetField("Description")
                );

                cmd.Parameters.AddWithValue(
                    "category",
                    csv.GetField("Category")
                );

                cmd.Parameters.AddWithValue(
                    "price",
                    price
                );

                cmd.Parameters.AddWithValue(
                    "rarity",
                    csv.GetField("Rarity")
                );

                cmd.Parameters.AddWithValue(
                    "viewCount",
                    int.Parse(csv.GetField("View_count"))
                );

                cmd.Parameters.AddWithValue(
                    "purchaseCount",
                    int.Parse(csv.GetField("Purchase_count"))
                );

                cmd.Parameters.AddWithValue(
                    "stock",
                    int.Parse(csv.GetField("Stock"))
                );

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Skipped row: {ex.Message}"
                );
            }
        }

        Console.WriteLine(
            "Products imported successfully."
        );
    }
}