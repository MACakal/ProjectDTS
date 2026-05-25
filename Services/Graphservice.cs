using Neo4j.Driver;

namespace ProjectDTS;

public class Graphservice : IDisposable
{
    private readonly IDriver _driver;

    public Graphservice(string uri, string user, string password)
    {
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
    }

    public List<Dictionary<string, object>> GetProductsBoughtTogether()
    {
        const string query = """
            MATCH (o:Order)-[:CONTAINS]->(p1:Product),
                  (o)-[:CONTAINS]->(p2:Product)
            WHERE p1.product_id < p2.product_id
            RETURN DISTINCT p1.name AS product1,
                   p2.name AS product2
            LIMIT 10
            """;
        return RunQuery(query);
    }

    public List<Dictionary<string, object>> GetSimilarCustomers()
    {
        const string query = """
            MATCH (c1:Customer)-[:PLACED]->(:Order)-[:CONTAINS]->(p:Product)
                  <-[:CONTAINS]-(:Order)<-[:PLACED]-(c2:Customer)
            WHERE c1.customer_id < c2.customer_id
            WITH DISTINCT c1, c2, COUNT(DISTINCT p) AS shared_count
            RETURN c1.first_name + ' ' + c1.last_name AS customer1,
                   c2.first_name + ' ' + c2.last_name AS customer2,
                   shared_count AS shared_products
            ORDER BY shared_count DESC
            LIMIT 10
            """;
        return RunQuery(query);
    }

    public List<Dictionary<string, object>> GetCategoryRetention()
    {
        const string query = """
            MATCH (c:Customer)-[:PLACED]->(o:Order)-[:CONTAINS]->(p:Product)-[:BELONGS_TO]->(cat:Category)
            RETURN DISTINCT c.first_name + ' ' + c.last_name AS customer,
                   cat.name AS category,
                   p.name AS product
            LIMIT 10
            """;
        return RunQuery(query);
    }

    public List<Dictionary<string, object>> GetProductRatings()
    {
        const string query = """
            MATCH (r:Review)-[:ABOUT]->(p:Product)
            RETURN p.name AS product,
                   AVG(r.rating) AS avg_rating,
                   COUNT(r) AS review_count
            ORDER BY avg_rating DESC
            LIMIT 15
            """;
        return RunQuery(query);
    }

    public List<Dictionary<string, object>> GetMostPurchasedProducts()
    {
        const string query = """
            MATCH (p:Product)-[:BELONGS_TO]->(cat:Category)
            RETURN DISTINCT p.name AS product,
                   cat.name AS category,
                   p.purchase_count AS times_purchased
            ORDER BY times_purchased DESC
            LIMIT 10
            """;
        return RunQuery(query);
    }

    private List<Dictionary<string, object>> RunQuery(string query)
    {
        var results = new List<Dictionary<string, object>>();

        using var session = _driver.AsyncSession();
        var cursor = session.RunAsync(query).GetAwaiter().GetResult();
        var records = cursor.ToListAsync().GetAwaiter().GetResult();

        foreach (var record in records)
        {
            var row = new Dictionary<string, object>();
            foreach (var key in record.Keys)
                row[key] = record[key].As<object>();
            results.Add(row);
        }

        return results;
    }

    public void Dispose() => _driver?.Dispose();
}