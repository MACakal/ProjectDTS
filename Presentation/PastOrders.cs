namespace ProjectDTS;

using Microsoft.Extensions.Configuration;
public class PastOrders
{
    private static readonly MongoDbContext _mongoContext = new MongoDbContext(
        new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
    );
    private static readonly UserActionLogService _userActionLogService = new UserActionLogService(_mongoContext);
    // private static BasketService _basketService = new BasketService(new DatabaseService());
    private static BasketService _basketService =
    new BasketService(
        new DatabaseService(),
        new OrderMongoService(_mongoContext),
        _userActionLogService
    );
    private static RatingService _ratingService = new RatingService
    (StackExchange.Redis.ConnectionMultiplexer.Connect(DotNetEnv.Env.GetString("REDIS_URL")));

    private static readonly ProductAuditLogService _auditLogService = new ProductAuditLogService(_mongoContext);
    private static ProductService _productService = new ProductService(new DatabaseService(), _ratingService, _auditLogService);

    public static void ShowPastOrders()
    {
        var (start, end) = AskForDateRange();
        Console.Clear();
        decimal total;

        List<OrderLine> items = _basketService.GetPastOrderLines(
            UserSession.CurrentUser.Id,
            start,
            end,
            out total
        );

        Console.WriteLine($"\nShowing orders from {start:dd-MM-yyyy} to {end:dd-MM-yyyy}\n");
        Console.WriteLine($"Amount spent: €{total:N2}");
        Console.WriteLine("--- 🛒 Past Orders ---");

        if (items.Count == 0)
        {
            Console.WriteLine("No orders have been placed in this period.");
        }

        // Display orders with index and order date
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            Console.WriteLine($"{i + 1}. {item.Name,-20} | {item.Quantity}x | €{item.Subtotal:N2} | Ordered: {item.OrderDate:dd-MM-yyyy HH:mm}");
        }

        Console.WriteLine("\nOptions:");
        Console.WriteLine("[1] View as a bar chart");
        Console.WriteLine("[2] Choose time period");
        Console.WriteLine("[3] Cancel an order (within 24h)");
        Console.WriteLine("[4] Review a purchased product");
        Console.WriteLine("[5] View order status history");
        Console.WriteLine("[0] Back");

        string input = Console.ReadLine();

        if (input == "1")
        {
            ShowBarChart(items);
        }
        else if (input == "2")
        {
            ShowPastOrders();
        }
        else if (input == "3")
        {
            CancelOrder(items);
        }
        else if (input == "4")
        {
            ReviewPurchasedProduct(items);
        }
        else if (input == "5")
        {
            ShowOrderStatusHistory(items);
        }
    }

    private static void CancelOrder(List<OrderLine> items)
    {
        var orders = items
            .GroupBy(i => i.OrderId)
            .Select(g => g.First())
            .ToList();

        Console.WriteLine("Select an order to cancel:");

        for (int i = 0; i < orders.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. Order #{orders[i].OrderId} | {orders[i].OrderDate:dd-MM-yyyy HH:mm}"
            );
        }

        if (!int.TryParse(Console.ReadLine(), out int index) ||
            index < 1 || index > orders.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var order = orders[index - 1];

        var timeSinceOrder = DateTime.Now - order.OrderDate;

        if (timeSinceOrder.TotalHours > 24)
        {
            Console.WriteLine("You can only cancel orders within 24 hours.");
            return;
        }

        Console.WriteLine($"Are you sure you want to cancel Order #{order.OrderId}? (y/n)");

        string confirm = Console.ReadLine()?.ToLower();

        if (confirm == "y")
        {
            _basketService.CancelOrder(order.OrderId);
            Console.WriteLine("Order canceled successfully!");
        }
        else
        {
            Console.WriteLine("Cancellation aborted.");
        }
    }

    public static void ShowBarChart(List<OrderLine> items)
    {
        Console.Clear();
        Console.WriteLine("--- 📊 Spending Chart ---\n");

        if (items.Count == 0)
        {
            Console.WriteLine("No data available.");
            Console.ReadKey();
            return;
        }

        var grouped = items
            .GroupBy(i => i.Name)
            .Select(g => new
            {
                Name = g.Key,
                Total = g.Sum(x => x.Subtotal)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        decimal max = grouped.Max(i => i.Total);

        foreach (var item in grouped)
        {
            int barLength = (int)(item.Total / max * 30);
            string bar = new string('█', barLength);

            Console.WriteLine($"{item.Name,-15} | {bar} €{item.Total:N2}");
        }

        Console.ReadKey();
    }

    public static (DateTime start, DateTime end) AskForDateRange()
    {
        Console.WriteLine("Enter start date (dd-MM-yyyy) or press ENTER for last month:");
        string startInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(startInput))
        {
            DateTime endDefault = DateTime.Now;
            DateTime startDefault = endDefault.AddMonths(-1);
            return (startDefault, endDefault);
        }

        DateTime start;
        while (!DateTime.TryParseExact(startInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out start))
        {
            Console.WriteLine("Invalid date, use format dd-MM-yyyy:");
            startInput = Console.ReadLine();
        }

        Console.WriteLine("Enter end date (dd-MM-yyyy):");
        string endInput = Console.ReadLine();

        DateTime end;
        while (!DateTime.TryParseExact(endInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out end))
        {
            Console.WriteLine("Invalid date, use format dd-MM-yyyy:");
            endInput = Console.ReadLine();
        }

        return (start, end);
    }

    private static void ShowOrderStatusHistory(List<OrderLine> items)
    {
        var orderMongoService = new OrderMongoService(_mongoContext);

        var allOrders = orderMongoService
            .GetOrdersByUserIdAsync(UserSession.CurrentUser.Id)
            .GetAwaiter()
            .GetResult();

        var distinctOrders = allOrders
            .Where(o => o.PostgresOrderId > 0)
            .Select(o => new
            {
                OrderId = o.PostgresOrderId,
                Date = o.CreatedAt
            })
            .OrderByDescending(o => o.Date)
            .ToList();

        if (distinctOrders.Count == 0)
        {
            Console.WriteLine("No orders found.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nSelect an order:");
        for (int i = 0; i < distinctOrders.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. Order #{distinctOrders[i].OrderId} — {distinctOrders[i].Date:dd-MM-yyyy HH:mm}"
            );
        }

        if (!int.TryParse(Console.ReadLine(), out int choice)
            || choice < 1
            || choice > distinctOrders.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.ReadKey();
            return;
        }

        int selectedOrderId = distinctOrders[choice - 1].OrderId;

        var doc = allOrders.FirstOrDefault(
            o => o.PostgresOrderId == selectedOrderId);

        if (doc == null || doc.StatusHistory == null || doc.StatusHistory.Count == 0)
        {
            Console.WriteLine("No status history found for this order.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== STATUS HISTORY — Order #{selectedOrderId} ===\n");
        Console.ResetColor();

        foreach (var entry in doc.StatusHistory)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{entry.Timestamp.ToLocalTime():dd-MM-yyyy HH:mm}  ");
            Console.ResetColor();
            Console.WriteLine(entry.StatusName);
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    private static void ReviewPurchasedProduct(List<OrderLine> items)
    {
        Console.WriteLine("Select a purchased product to review:");

        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i].Name}");
        }

        if (!int.TryParse(Console.ReadLine(), out int choice) ||
            choice < 1 || choice > items.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var selectedItem = items[choice - 1];
        var product = _productService.GetById(selectedItem.ProductId);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        var viewProductPres = new ViewProductPres(_productService, _ratingService, _userActionLogService);

        viewProductPres.RateProduct(product, UserSession.CurrentUser.Id);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}