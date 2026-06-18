namespace UnitTests;

[TestClass]
public class SaveOrdersMongoStoryTests
{
    [DataTestMethod]
    [DataRow(12, 34, 49.95, 1, "Product", 3, 16.65, "Placed")]
    [DataRow(22, 44, 19.99, 2, "Other product", 1, 19.99, "Processing")]
    public void SaveOrdersMongo_ShouldRepresentOrderWithProductsAndStatusHistory(
        int userId,
        int postgresOrderId,
        double totalPrice,
        int productId,
        string productName,
        int quantity,
        double productPrice,
        string statusName)
    {
        var order = new OrderDocument
        {
            UserId = userId,
            PostgresOrderId = postgresOrderId,
            CreatedAt = DateTime.UtcNow,
            TotalPrice = Convert.ToDecimal(totalPrice),
            Products = new List<BasketItem>
            {
                new() { ProductId = productId, Name = productName, Quantity = quantity, Price = Convert.ToDecimal(productPrice) }
            },
            StatusHistory = new List<OrderStatusEntry>
            {
                new() { StatusName = statusName, Timestamp = DateTime.UtcNow }
            }
        };

        Assert.AreEqual(userId, order.UserId);
        Assert.AreEqual(postgresOrderId, order.PostgresOrderId);
        Assert.AreEqual(Convert.ToDecimal(totalPrice), order.TotalPrice);
        Assert.AreEqual(1, order.Products.Count);
        Assert.AreEqual(statusName, order.StatusHistory.Single().StatusName);
    }

    [DataTestMethod]
    [DataRow("SaveOrderSnapshot")]
    [DataRow("SaveOrderAsync")]
    [DataRow("StatusName = \"Placed\"")]
    public void SaveOrdersMongo_ShouldSaveSnapshotAfterTransactionalCheckout(string expectedCode)
    {
        var basketServiceText = File.ReadAllText(GetProjectFile("Services", "BasketService.cs"));

        StringAssert.Contains(basketServiceText, expectedCode);
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
