using Neo4j.Driver;

namespace ProjectDTS;

public class GraphProductService
{
    private readonly IDriver _driver;
    private readonly ProductService _productService;

    public GraphProductService(IDriver driver, ProductService productService)
    {
        _driver = driver;
        _productService = productService;
    }

    public async Task SyncProductsToGraph()
    {
        var products = _productService.GetAllProducts();

        await using var session = _driver.AsyncSession();

        foreach (var product in products)
        {
            var query = @"
            MERGE (c:Category {name:$category})

            MERGE (p:Product {product_id:$id})

            SET p.name=$name,
                p.price=$price,
                p.rarity=$rarity

            MERGE (p)-[:BELONGS_TO]->(c)
            ";

            await session.RunAsync(query, new
            {
                id = product.Id,
                name = product.Name,
                category = product.Category,
                price = product.Price,
                rarity = product.Rarity
            });
        }
    }
    public async Task<List<Product>> GetRelatedProducts(int productId)
    {
        await using var session = _driver.AsyncSession();

        var query = @"
    MATCH (p:Product {product_id:$id})
          -[:BELONGS_TO]->
          (c:Category)
          <-[:BELONGS_TO]-
          (related:Product)

    WHERE related.product_id <> p.product_id

    WITH related, rand() AS r
    ORDER BY r

    RETURN related
    LIMIT 3
    ";

        var result = await session.RunAsync(
            query,
            new { id = productId }
        );

        var products = new List<Product>();

        await foreach (var record in result)
        {
            var node = record["related"].As<INode>();

            products.Add(new Product
            {
                Id = node.Properties["product_id"].As<int>(),
                Name = node.Properties["name"].As<string>(),
                Price = node.Properties["price"].As<decimal>(),
                Rarity = node.Properties["rarity"].As<string>()
            });
        }
        // await foreach (var record in result)
        // {
        //     var node = record["related"].As<INode>();

        //     Console.WriteLine(
        //         $"Found product: {node.Properties["name"]}"
        //     );

        //     products.Add(new Product
        //     {
        //         Id = node.Properties["product_id"].As<int>(),
        //         Name = node.Properties["name"].As<string>(),
        //         Price = node.Properties["price"].As<decimal>(),
        //         Rarity = node.Properties["rarity"].As<string>()
        //     });
        // }

        Console.WriteLine($"Total found: {products.Count}");

        return products;
    }
}