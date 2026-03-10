using Npgsql;
using NpgsqlTypes;
namespace ProjectDTS;
public class BasketMenu
{
    
    private static ProductService _productService = new ProductService(new DatabaseService());
    private static FilterMenu _filterMenu = new FilterMenu(_productService); 
    private static BasketService _basketService = new BasketService(new DatabaseService());
    public BasketMenu(ProductService productService, FilterMenu filterMenu, BasketService basketService)
    {
        _productService = productService;
        _filterMenu = filterMenu;
        _basketService = basketService;
    }
    public static void WhatToDo()
    {
        Console.WriteLine();
        Console.WriteLine("What do you want to do?");
        Console.WriteLine("1. Add product to basket");
        Console.WriteLine("2. Filter products");
        Console.WriteLine("3. View Basket");
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
            case "3":
                Console.Clear();
                ShowBasket();
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
        if (UserSession.CurrentUser == null)
        {
            Console.WriteLine("⚠️ You must be logged in to add products to your basket!");
            return;
        }
        PrintAll();
        Console.WriteLine();
        Console.WriteLine("\n--- Add Product to Basket ---");
        Console.Write("Enter product ID: ");
        int productId = int.Parse(Console.ReadLine());
        Console.Write("How many? ");
        int quantity = int.Parse(Console.ReadLine());
        //AddToBasket();
        _basketService.AddToBasket(UserSession.CurrentUser.Id, productId, quantity);
        Console.WriteLine($"Added {quantity} x product {productId} to your basket.");
    }

    public static void ShowBasket()
    {
        Console.Clear();
        if (UserSession.CurrentUser == null) return;

        var items = _basketService.GetBasketLines(UserSession.CurrentUser.Id, out decimal totalPrice);

        Console.WriteLine("--- 🛒 Your Shopping Cart ---");
        if (items.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
        }
        else
        {
            foreach (var line in items) Console.WriteLine(line);
            Console.WriteLine("------------------------------");
            Console.WriteLine($"Total Amount: €{totalPrice:N2}");
            
            Console.WriteLine("\n1. 💳 Pay Now (Checkout)");
        }
        Console.WriteLine("0. Back");

        var choice = Console.ReadLine();
        if (choice == "1" && items.Count > 0)
        {
            Console.Write("Confirm payment? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                if (_basketService.CheckoutWithTransaction(UserSession.CurrentUser.Id))
                {
                    Console.WriteLine("\n✅ Payment successful! Thank you for your purchase.");
                }
                else
                {
                    Console.WriteLine("\n❌ Payment failed. Please try again.");
                }
                Console.ReadKey();
            }
        }
    }

    public static void PrintAll()
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
    }
}