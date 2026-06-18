namespace UnitTests;

[TestClass]
public class SaveOrdersMongoStoryTests
{
    [TestMethod]
    public void SaveOrdersMongo_ShouldRepresentOrderWithProductsAndStatusHistory()
    {
        var order = new OrderDocument
        {
            UserId = 12,
            PostgresOrderId = 34,
            CreatedAt = DateTime.UtcNow,
            TotalPrice = 49.95m,
            Products = new List<BasketItem>
            {
                new() { ProductId = 1, Name = "Product", Quantity = 3, Price = 16.65m }
            },
            StatusHistory = new List<OrderStatusEntry>
            {
                new() { StatusName = "Placed", Timestamp = DateTime.UtcNow }
            }
        };

        Assert.AreEqual(12, order.UserId);
        Assert.AreEqual(34, order.PostgresOrderId);
        Assert.AreEqual(49.95m, order.TotalPrice);
        Assert.AreEqual(1, order.Products.Count);
        Assert.AreEqual("Placed", order.StatusHistory.Single().StatusName);
    }

    [TestMethod]
    public void SaveOrdersMongo_ShouldSaveSnapshotAfterTransactionalCheckout()
    {
        var basketServiceText = File.ReadAllText(GetProjectFile("Services", "BasketService.cs"));

        StringAssert.Contains(basketServiceText, "SaveOrderSnapshot");
        StringAssert.Contains(basketServiceText, "SaveOrderAsync");
    }

    private static string GetProjectFile(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "ProjectDTS.csproj")))
        {
            directory = directory.Parent;
        }

        Assert.IsNotNull(directory, "Could not find project root.");
        return Path.Combine(new[] { directory!.FullName }.Concat(parts).ToArray());
    }
}
