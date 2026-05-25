using Neo4j.Driver;

namespace ProjectDTS;

public class Neo4jImportService
{
    private readonly GraphDatabaseService _graphDb;

    public Neo4jImportService(
        GraphDatabaseService graphDb)
    {
        _graphDb = graphDb;
    }

    public async Task ShowProductsAsync()
    {
        var session =
            _graphDb.Driver.AsyncSession();

        var result =
            await session.RunAsync(@"
                MATCH (p:Product)
                RETURN p
            ");

        await result.ForEachAsync(record =>
        {
            var p = record["p"].As<INode>();

            var name =
                p.Properties.ContainsKey("name")
                ? p["name"]
                : "No name";

            var price =
                p.Properties.ContainsKey("price")
                ? p["price"]
                : "No price";

            Console.WriteLine(
                $"{name} - {price}"
            );
        });

        await session.CloseAsync();
    }
}