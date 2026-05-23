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

            MERGE (p:Product {id:$id})

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
}