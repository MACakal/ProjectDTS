using ProjectDTS;

namespace UnitTests;

[TestClass]
public class LargeDataPerformanceStoryTests
{
    [TestMethod]
    public void LargeDataPerformance_ShouldProcessLargeProductSetQuickly()
    {
        var products = CreateProducts(10_000);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var groupedStock = products
            .GroupBy(p => p.Category)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Stock));

        stopwatch.Stop();

        Assert.AreEqual(10_000, products.Count);
        Assert.AreEqual(5, groupedStock.Count);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, $"Processing took {stopwatch.ElapsedMilliseconds}ms.");
    }

    [TestMethod]
    public void LargeDataPerformance_ShouldSortLargeProductSetQuickly()
    {
        var products = CreateProducts(10_000);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var topProducts = products
            .OrderByDescending(p => p.Price)
            .Take(100)
            .ToList();

        stopwatch.Stop();

        Assert.AreEqual(100, topProducts.Count);
        Assert.AreEqual(10_000m, topProducts[0].Price);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, $"Sorting took {stopwatch.ElapsedMilliseconds}ms.");
    }

    private static List<Product> CreateProducts(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Product
            {
                Id = i,
                Name = $"Product {i}",
                Category = $"Category {i % 5}",
                Price = i,
                Stock = i % 100
            })
            .ToList();
    }
}
