using ProjectDTS;

namespace UnitTests;

[TestClass]
public class LargeDataPerformanceStoryTests
{
    [DataTestMethod]
    [DataRow(1_000)]
    [DataRow(10_000)]
    [DataRow(25_000)]
    public void LargeDataPerformance_ShouldProcessLargeProductSetQuickly(int productCount)
    {
        var products = CreateProducts(productCount);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var groupedStock = products
            .GroupBy(p => p.Category)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Stock));

        stopwatch.Stop();

        Assert.AreEqual(productCount, products.Count);
        Assert.AreEqual(5, groupedStock.Count);
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, $"Processing took {stopwatch.ElapsedMilliseconds}ms.");
    }

    [DataTestMethod]
    [DataRow(1_000, 25)]
    [DataRow(10_000, 100)]
    [DataRow(25_000, 250)]
    public void LargeDataPerformance_ShouldSortLargeProductSetQuickly(int productCount, int take)
    {
        var products = CreateProducts(productCount);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var topProducts = products
            .OrderByDescending(p => p.Price)
            .Take(take)
            .ToList();

        stopwatch.Stop();

        Assert.AreEqual(take, topProducts.Count);
        Assert.AreEqual(productCount, topProducts[0].Price);
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
