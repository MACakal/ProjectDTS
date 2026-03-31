using ProjectDTS;

namespace ProjectDTS;

public class FilterMenu
{
    private readonly ProductService _productService;
    private readonly ViewProductPres _viewProductPres;


    public FilterMenu(ProductService productService, ViewProductPres viewProductPres)
    {
        _productService = productService;
        _viewProductPres = viewProductPres;
    }


    public void Show()
    {
        while (true)
        {
            System.Console.WriteLine("1. Category");
            System.Console.WriteLine("2. Price");
            System.Console.WriteLine("3. Price range");
            System.Console.WriteLine("4. Search by Name");
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
                case "0":
                    {
                        Console.Clear();
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

        Console.WriteLine($"\nResults for '{cat}':");
        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id} - {p.Name} - €{p.Price}");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadLine();
        Console.Clear();
    }

    public void PriceRangeFilter()
    {
        decimal min = ReadDecimal("Minimum?", x => true, "Invalid number.");
        decimal max = ReadDecimal("Maximum?", x => x >= min, "Max must be greater than min.");
        var products = _productService.GetProductsByRange(min, max);
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
        bool ascending = (sortChoice == "1");

        var results = _productService.GetProductsSortedByPrice(ascending);

        Console.WriteLine("\n---- Sorted Products ----");
        foreach (var p in results)
        {
            Console.WriteLine($"{p.Id} - {p.Name} - €{p.Price}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadLine();
        Console.Clear();
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

        Console.WriteLine($"\n---- Search results for '{input}' ----");
        if (results.Count == 0)
        {
            Console.WriteLine("No products found matching that name.");
        }
        else
        {
            foreach (var p in results)
            {
                Console.WriteLine($"{p.Id} - {p.Name} - €{p.Price}");
            }
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadLine();
        Console.Clear();

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
}