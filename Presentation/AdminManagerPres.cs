namespace ProjectDTS;

public class AdminManagerPres
{
    private UserService _userService;
    private RatingService _ratingService;

    private static ProductService _service = new ProductService(new DatabaseService());

    private NotificationService _notificationService = new NotificationService(new DatabaseService());
    private static ViewProductPres _viewService = new ViewProductPres(_service);
    public AdminManagerPres(UserService userService, RatingService ratingService)
    {
        _userService = userService;
        _ratingService = ratingService;
    }
    public Product? CreateProduct()
    {

        while (true)
        {

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Press 'q' to exit");
            Console.ResetColor();

            Console.WriteLine("Enter product name:");
            var name = Console.ReadLine();
            if (name.ToLower() == "q") return null;

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Press 'q' to exit");
            Console.ResetColor();

            Console.WriteLine("Enter description:");
            var description = Console.ReadLine();
            if (description.ToLower() == "q") return null;

            // Console.WriteLine("Choose category:");
            // while (true)
            // {

            var category = ChooseCategory();
            // if (category == null)
            // {
            //     Console.WriteLine("Invalid input, try again");
            // }
            // else
            // {
            //     break;
            // }
            // }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Press 'q' to exit");
            Console.ResetColor();

            Console.WriteLine("Enter price:");
            decimal price;
            while (true)
            {
                var input = Console.ReadLine();
                if (input?.ToLower() == "q")
                    return null;
                if (decimal.TryParse(input, out price) && price > 0)
                {
                    break;
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Invalid price. Enter a valid number:");
                Console.ResetColor();

            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Press 'q' to exit");
            Console.ResetColor();

            Console.WriteLine("Enter rarity:");
            var rarity = Console.ReadLine();
            if (rarity.ToLower() == "q") return null;

            while (true)
            {
                Console.WriteLine("Save this product? (y/n)");
                var answer = Console.ReadLine().ToLower();

                if (answer == "y")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    string message = "Saving...";

                    foreach (char c in message)
                    {
                        Console.Write(c);
                        Thread.Sleep(100);
                    }
                    Console.ResetColor();
                    Console.WriteLine();
                    break;
                }

                if (answer == "n")
                {

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Product not saved.");
                    Console.ResetColor();
                    return null;
                }

                Console.WriteLine("Please enter y or n");
            }


            {


                return new Product
                {
                    Name = name,
                    Description = description!,
                    Category = category!,
                    Price = price,
                    Rarity = rarity!
                };
            }

        }

    }

    public Product? EditProduct()
    {
        var db = new DatabaseService();
        var productService = new ProductService(db);

        Console.Clear();

        var products = productService.GetAllProducts();

        Console.WriteLine("\n---- Product List ----");

        if (products.Count == 0)
        {
            Console.WriteLine("There are no products to show.");
            return null;
        }

        Console.WriteLine($"{"ID",-5} {"Name",-20} {"Category",-20} {"Price",10}");
        Console.WriteLine(new string('-', 60));

        foreach (var p in products.OrderBy(p => p.Id))
        {
            Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Category,-20} {p.Price,10}€");
        }


        Product? product = null;

        while (product == null)
        {
            Console.WriteLine("Enter product id:");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine("Invalid number, try again.");
                continue;
            }

            product = productService.GetById(id);

            if (product == null)
            {
                Console.WriteLine("Product not found, try again.");
            }
        }

        bool editing = true;

        while (editing)
        {
            Console.Clear();

            Console.WriteLine("Editing product:");
            Console.WriteLine($"Name: {product.Name}");
            Console.WriteLine($"Description: {product.Description}");
            Console.WriteLine($"Category: {product.Category}");
            Console.WriteLine($"Price: {product.Price}");
            Console.WriteLine($"Rarity: {product.Rarity}");

            Console.WriteLine();
            Console.WriteLine("1 Change name");
            Console.WriteLine("2 Change description");
            Console.WriteLine("3 Change category");
            Console.WriteLine("4 Change price");
            Console.WriteLine("5 Change rarity");
            Console.WriteLine("6 Save and exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter new name:");
                    product.Name = Console.ReadLine();
                    break;

                case "2":
                    Console.WriteLine("Enter new description:");
                    product.Description = Console.ReadLine();
                    break;

                case "3":
                    product.Category = ChooseCategory();
                    break;

                case "4":
                    Console.WriteLine("Enter new price:");

                    decimal price;
                    while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
                    {
                        Console.WriteLine("Invalid price. Enter a valid number:");
                    }

                    product.Price = price;
                    break;

                case "5":
                    Console.WriteLine("Enter new rarity:");
                    product.Rarity = Console.ReadLine();
                    break;

                case "6":
                    editing = false;
                    break;
            }
        }

        productService.UpdateProduct(product);
        Console.ForegroundColor = ConsoleColor.Green;
        string message = "Saving...";

        foreach (char c in message)
        {
            Console.Write(c);
            Thread.Sleep(100);
        }
        Console.WriteLine();
        Console.ResetColor();
        Console.WriteLine("Product updated successfully!");

        return product;
    }

    public static string? ChooseCategory()
    {
        Console.WriteLine();

        string[] categories =
        {
        "Electronics",
        "Books",
        "Games",
        "Toys",
        "Home & Kitchen",
        "Clothing",
        "Sports",
        "Beauty",
        "Office",
        "Pet Supplies"
    };

        while (true)
        {
            Console.WriteLine("Choose a category:");

            for (int i = 0; i < categories.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            var input = Console.ReadLine(); // string
            int choice;                     // int

            if (!int.TryParse(input, out choice)
                || choice < 1
                || choice > categories.Length)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input, please try again");
                Console.ResetColor();
            }
            else
            {
                return categories[choice - 1];
            }
        }
    }

    public void MostPopularCategories()
    {
        Console.Clear();

        var (start, end) = PastOrders.AskForDateRange();
        var cats = _service.GetPopularCategories(start, end);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== MOST POPULAR CATEGORIES ===");
        Console.ResetColor();

        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{"Rank",-6} {"Category",-20} {"Purchases",10}");
        Console.ResetColor();

        Console.WriteLine(new string('-', 40));

        int rank = 1;

        foreach (var c in cats)
        {
            Console.WriteLine($"{rank,-6} {c.Category,-20} {c.TotalPurchases,10}");
            rank++;
        }

        Console.WriteLine();
        Console.WriteLine(new string('-', 40));
        Console.WriteLine("Options:");
        Console.WriteLine("[1] View as diagram ");
        Console.WriteLine("[0] Back");

        string input = Console.ReadLine();

        if (input == "1")
        {
            var chartData = cats.Select(c => (c.Category, c.TotalPurchases)).ToList();
            ShowCategoryBarChart(chartData);
            MostPopularCategories();
        }
    }

    public void ShowCategoryBarChart(List<(string Category, int TotalPurchases)> cats)
    {
        Console.Clear();
        Console.WriteLine("--- 📊 Most Popular Categories ---\n");

        if (cats.Count == 0)
        {
            Console.WriteLine("No data available.");
            Console.ReadKey();
            return;
        }

        int max = cats.Max(c => c.TotalPurchases);

        foreach (var c in cats)
        {
            int barLength = (int)((double)c.TotalPurchases / max * 30); // max 30 blokjes
            string bar = new string('█', barLength);

            Console.WriteLine($"{c.Category,-20} | {bar} {c.TotalPurchases}");
        }

        Console.ReadKey();
    }



    public void ShowUserSpending()
    {
        var users = _userService.GetUserSpending();
        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("ID   | Name           | Total Spending");
        Console.ResetColor();

        Console.WriteLine("----------------------------------------");

        foreach (var user in users)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{user.Id,-4} | ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{user.Name,-14} | ");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{user.TotalSpending,10}");

            Console.ResetColor();
        }
    }

    public void ShowNotifications()
    {
        var notifications = _notificationService.GetNotifications();

        if (notifications.Count == 0)
        {
            Console.WriteLine("No notifications found.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("ID   | Message");
        Console.ResetColor();

        Console.WriteLine("----------------------------------------");

        foreach (var notification in notifications)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{notification.Id,-4} | ");

            if (notification.IsRead)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.WriteLine($"{notification.Message}");

            Console.ResetColor();
        }

        _notificationService.MarkAllAsRead();
    }


    public void HandleDeleteProduct()
    {
        var products = _service.GetAllProducts();
        _viewService.DisplayProducts(products);
        Console.Write("Enter product id to delete: ");

        if (int.TryParse(Console.ReadLine(), out int id))
        {
            try
            {

                bool deleted = _service.DeleteProduct(id);
                if (!deleted)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Product Not Found");
                    Console.ResetColor();
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Product deleted successfully");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid id");
            Console.ResetColor();

        }
    }


    public void HandleDeleteReview()
    {
        Console.Clear();

        var reviews = _ratingService.GetAllRatingsWithDetails();

        if (reviews.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No reviews found.");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== ALL REVIEWS ===\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{"ID",-5} {"Product",-20} {"User",-15} {"Stars",-7} {"Date",-12} Review");
        Console.ResetColor();
        Console.WriteLine(new string('-', 80));

        foreach (var r in reviews)
        {
            string stars = new string('*', r.RatingValue);
            string reviewSnippet = r.ReviewText?.Length > 25 ? r.ReviewText[..25] + "..." : (r.ReviewText ?? "-");
            Console.WriteLine($"{r.RatingId,-5} {r.ProductName,-20} {r.UserName,-15} {stars,-7} {r.CreatedAt:yyyy-MM-dd,-12} {reviewSnippet}");
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Press 'q' to cancel");
        Console.ResetColor();
        Console.Write("Enter review ID to delete: ");

        var input = Console.ReadLine();
        if (input?.ToLower() == "q") return;

        if (!int.TryParse(input, out int id) || reviews.All(r => r.RatingId != id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid review ID.");
            Console.ResetColor();
            return;
        }

        var review = reviews.First(r => r.RatingId == id);
        Console.WriteLine($"\nDelete review by \"{review.UserName}\" on \"{review.ProductName}\"? (y/n)");

        if (Console.ReadLine()?.ToLower() != "y") return;

        _ratingService.DeleteRating(id);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Review deleted successfully.");
        Console.ResetColor();
    }

    public void ShowTopProductsPerCategory()
    {
        var products = _service.GetProductsPerCategory();

        var grouped = products.GroupBy(p => p.Category);

        foreach (var group in grouped)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n=== {group.Key} ===");
            Console.ResetColor();

            foreach (var p in group)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{p.Name,-25}");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" | ");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{p.Price,10}");

                Console.ResetColor();
            }
        }

    }

    public void ViewUsers()
    {
        var users = _userService.GetAllUsers();

        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id} | Name: {user.Name} | Role: {user.Role}");
        }
    }

    public void EditUser()
    {
        Console.Clear();
        var users = _userService.GetAllUsers();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (var u in users)
        {
            Console.WriteLine($"ID: {u.Id} | Name: {u.Name} | Role: {u.Role}");
        }

        Console.WriteLine("\nEnter user ID to edit:");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var user = _userService.GetUserById(id);

        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.WriteLine($"Editing user: {user.Name}");

        Console.Write("New name (leave empty to keep current): ");
        var name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name))
        {
            user.Name = name;
        }

        Console.Write("New role (Admin/User, leave empty to keep current): ");
        var roleInput = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(roleInput))
        {
            if (Enum.TryParse(roleInput, true, out UserRole role))
            {
                user.Role = role;
            }
            else
            {
                Console.WriteLine("Invalid role, keeping old value.");
            }
        }

        _userService.UpdateUser(user);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("User updated successfully!");
        Console.ResetColor();
    }

    public void DeleteUser()
    {
        Console.Clear();
        var users = _userService.GetAllUsers();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (var u in users)
        {
            Console.WriteLine($"ID: {u.Id} | Name: {u.Name} | Role: {u.Role}");
        }

        Console.WriteLine("\nEnter user ID to delete:");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var user = _userService.GetUserById(id);

        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        // 🔥 Important safety check
        if (user.Id == UserSession.CurrentUser?.Id)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You cannot delete your own account.");
            Console.ResetColor();
            return;
        }

        Console.Write($"Are you sure you want to delete {user.Name}? (y/n): ");
        var confirm = Console.ReadLine()?.ToLower();

        if (confirm == "y")
        {
            _userService.DeleteUser(id);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("User deleted successfully.");
            Console.ResetColor();
        }
    }


}

