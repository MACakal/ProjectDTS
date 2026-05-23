using ProjectDTS;

namespace ProjectDTS;

public class FilterMenu
{
    private readonly ProductService _productService;
    private readonly ViewProductPres _viewProductPres;
    private readonly UserActionLogService _userActionLogService;


    public FilterMenu(ProductService productService, ViewProductPres viewProductPres, UserActionLogService userActionLogService)
    {
        _productService = productService;
        _viewProductPres = viewProductPres;
        _userActionLogService = userActionLogService;
    }


   public void Show()
    {
        while (true)
        {
            System.Console.WriteLine("1. Category");
            System.Console.WriteLine("2. Price");
            System.Console.WriteLine("3. Price range");
            System.Console.WriteLine("4. Search by Name");
            System.Console.WriteLine("5. Filter by Reviews");
            System.Console.WriteLine("0. Back");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    {
                        Console.Clear();
                        PrintAllwithCategories();
                        CategoryFilter();
                        break;
                    }
                case "2":
                    {
                        Console.Clear();
                        PrintAll();
                        PriceFilter();
                        break;
                    }
                case "3":
                    {
                        Console.Clear();
                        PriceRangeFilter();
                        break;
                    }
                case "4":
                    {
                        Console.Clear();
                        PrintAll();
                        SearchByName();
                        break;
                    }
                case "5":
                    {
                        Console.Clear();
                        ReviewFilter();
                        break;
                    }
                case "0":
                    {
                        return;
                    }
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }


    public void CategoryFilter()
    {
        Console.WriteLine("\nWhat category do you want to filter by?");
        var cat = Console.ReadLine() ?? "";
        var products = _productService.GetProductsByCategory(cat);

        _ = _userActionLogService.SaveUserActionLogAsync(new UserActionLog
        {
            UserSessionId = UserSession.SessionId,
            UserId = null,
            ActionType = "FilterByCategory",
            Details = new Dictionary<string, string>
            {
                { "Category", cat },
                { "ResultCount", products.Count().ToString() }
            }
        });

        _viewProductPres.Viewproducts(products);
    }

    public void PriceRangeFilter()
    {
        decimal min = ReadDecimal("Minimum?", x => true, "Invalid number.");
        decimal max = ReadDecimal("Maximum?", x => x >= min, "Max must be greater than min.");
        var products = _productService.GetProductsByRange(min, max);
        _ = _userActionLogService.SaveUserActionLogAsync(new UserActionLog
        {
            UserSessionId = UserSession.SessionId,
            UserId = null,
            ActionType = "FilterByPriceRange",
            Details = new Dictionary<string, string>
            {
                { "Minimum", min.ToString() },
                { "Maximum", max.ToString() },
                { "ResultCount", products.Count().ToString() }
            }
        });
        _viewProductPres.Viewproducts(products);
    }

    public decimal ReadDecimal(string message, Func<decimal, bool> validator, string errorMessage)
    {
        decimal value;

        while (true)
        {
            Console.WriteLine(message);
            if (decimal.TryParse(Console.ReadLine(), out value) && validator(value))
                return value;

            Console.WriteLine(errorMessage);
        }
    }

    public void PriceFilter()
    {
        Console.WriteLine("\nHow do you want to sort?");
        Console.WriteLine("1. Cheapest first (Low to High)");
        Console.WriteLine("2. Most expensive first (High to Low)");

        var sortChoice = Console.ReadLine();
        bool ascending = sortChoice == "1";

        var results = _productService.GetProductsSortedByPrice(ascending);
        _ = _userActionLogService.SaveUserActionLogAsync(new UserActionLog
        {
            UserSessionId = UserSession.SessionId,
            UserId = null,
            ActionType = "FilterByPrice",
            Details = new Dictionary<string, string>
            {
                { "SortOrder", ascending ? "LowToHigh" : "HighToLow" },
                { "ResultCount", results.Count().ToString() }
            }
        });
        Console.Clear();
        _viewProductPres.Viewproducts(results);
    }
    public void SearchByName()
    {
        Console.WriteLine("\nEnter the name of the product to search for:");
        var input = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Search cannot be empty.");
            return;
        }

        var results = _productService.SearchProductsByName(input);

        _ = _userActionLogService.SaveUserActionLogAsync(new UserActionLog
        {
            UserSessionId = UserSession.SessionId,
            UserId = null,
            ActionType = "SearchProducts",
            Details = new Dictionary<string, string>
            {
                { "SearchTerm", input },
                { "ResultCount", results.Count().ToString() }
            }
        });
        _viewProductPres.Viewproducts(results);

    }

    public void PrintAll()
    {
        var products = _productService.GetAllProducts();
        Console.WriteLine("\n---- Product List ----");
        Console.WriteLine($"ID ---------- Name --------------- Price");
        foreach (var p in products)
        {
            if (p != null)
            {
                Console.WriteLine($"{p.Id,-5} {p.Name,-20} €{p.Price,13}");
            }
            else
            {
                Console.WriteLine("No products available.");
                return;
            }
        }
    }

    public void PrintAllwithCategories()
    {
        var products = _productService.GetAllProducts();
        Console.WriteLine("\n---- Product List ----");
        Console.WriteLine($"ID ---------- Name ---------------Price ------------ Category");
        foreach (var p in products)
        {
            if (p != null)
            {
                Console.WriteLine($"{p.Id,-5} {p.Name,-20} €{p.Price,13}  {p.Category,16}");
            }
            else
            {
                Console.WriteLine("No products available.");
                return;
            }
        }
    }

    public void ReviewFilter()
    {
        Console.WriteLine("\nFilter by star rating:");
        Console.WriteLine("1. 1 Star");
        Console.WriteLine("2. 2 Stars");
        Console.WriteLine("3. 3 Stars");
        Console.WriteLine("4. 4 Stars");
        Console.WriteLine("5. 5 Stars");

        string input = Console.ReadLine();

        if (!int.TryParse(input, out int stars) || stars < 1 || stars > 5)
        {
            Console.WriteLine("Invalid option.");
            return;
        }

        var products = _productService.GetProductsByStarRating(stars);

        _ = _userActionLogService.SaveUserActionLogAsync(new UserActionLog
        {
            UserId = null,
            UserSessionId = UserSession.SessionId,
            ActionType = "FilterByStarRating",
            Details = new Dictionary<string, string>
            {
                { "StarRating", stars.ToString() },
                { "ResultCount", products.Count().ToString() }
            }
        });

        Console.Clear();
        _viewProductPres.Viewproducts(products);
    }
}