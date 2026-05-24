using ProjectDTS;
public class GraphInfoLogic
{
    private readonly Graphservice _neo4jService;

    public GraphInfoLogic(Graphservice neo4jService)
    {
        _neo4jService = neo4jService;
    }

    public List<(string Product1, string Product2)> GetProductsBoughtTogether()
    {
        var results = _neo4jService.GetProductsBoughtTogether();
        return results.Select(r => (
            r["product1"].ToString()!,
            r["product2"].ToString()!
        )).ToList();
    }

    public List<(string Customer1, string Customer2, string SharedProduct)> GetSimilarCustomers()
    {
        var results = _neo4jService.GetSimilarCustomers();
        return results.Select(r => (
            r["customer1"].ToString()!,
            r["customer2"].ToString()!,
            r["shared_product"].ToString()!
        )).ToList();
    }

    public List<(string Customer, string Category, string Product)> GetCategoryRetention()
    {
        var results = _neo4jService.GetCategoryRetention();
        return results.Select(r => (
            r["customer"].ToString()!,
            r["category"].ToString()!,
            r["product"].ToString()!
        )).ToList();
    }

    public List<(string Product, double AvgRating, int ReviewCount)> GetProductRatings()
    {
        var results = _neo4jService.GetProductRatings();
        return results.Select(r => (
            r["product"].ToString()!,
            Convert.ToDouble(r["avg_rating"]),
            Convert.ToInt32(r["review_count"])
        )).ToList();
    }

    public List<(string Product, string Category, int TimesPurchased)> GetMostPurchasedProducts()
    {
        var results = _neo4jService.GetMostPurchasedProducts();
        return results.Select(r => (
            r["product"].ToString()!,
            r["category"].ToString()!,
            Convert.ToInt32(r["times_purchased"])
        )).ToList();
    }
}