using Npgsql;
using NpgsqlTypes;
namespace ProjectDTS;
public class BasketMenu
{
    
    static ProductService _productService = new ProductService(new DatabaseService());
    private static FilterMenu _filterMenu;

    public BasketMenu(ProductService productService, FilterMenu filterMenu)
    {
        _productService = productService;
        _filterMenu = filterMenu;
    }
    public static void WhatToDo()
    {
        Console.WriteLine();
        Console.WriteLine("What do you want to do?");
        Console.WriteLine("1. Add product to basket");
        Console.WriteLine("2. Filter products");
        Console.WriteLine("0. Back");

        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Console.Clear();
                AddToBasketPrint();
                break;
            case "2":
                Console.Clear();
                _filterMenu.Show();
                break;
            case "0":
                Console.Clear();
                return;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    public static void AddToBasketPrint()
    {
        var products = _productService.GetAllProducts();
        Console.WriteLine("\n---- Product List ----");
        Console.WriteLine($"ID ---------- Name -------------------- Price");
        foreach (var p in products)
        {
            if(p != null)
            {
                Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Price,10}€");
            }
            else
            {
                Console.WriteLine("No products available.");
                return;
            }
        }
        Console.WriteLine();
        Console.WriteLine("\n--- Add Product to Basket ---");
        Console.Write("Enter product ID: ");
        int productId = int.Parse(Console.ReadLine());
        Console.Write("How many? ");
        int quantity = int.Parse(Console.ReadLine());
        Console.WriteLine($"Added {quantity} x product {productId} to your basket.");
    }

    public static void ShowBasket()
    {
        Console.WriteLine("\n--- Basket Menu ---");
        Console.WriteLine("1. View Basket");
        Console.WriteLine("2. Checkout");
        Console.WriteLine("0. Back");

        var choice = Console.ReadLine();
        if (choice == "1")
        {
            Console.Clear();
            // Implement view basket functionality
            Console.WriteLine("Viewing basket...");
            Console.WriteLine("\nPress any key to return to the basket menu.");
            Console.ReadLine();
            Console.Clear();
        }
        else if (choice == "2")
        {
            Console.Clear();
            //payd to true
            Console.WriteLine("Checking out...");
            Console.WriteLine("\nPress any key to return to the basket menu.");
            Console.ReadLine();
            Console.Clear();
        }
        else if (choice == "0")
        {
            Console.Clear();
            return;
        }
    }
}