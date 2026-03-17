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
        while (true) // Zorgt dat je in dit menu blijft tot je '0' kiest
        {
            Console.WriteLine("\nWhat do you want to do?");
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
                    return; // Gaat terug naar het vorige scherm

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
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

    public static void printBasket()
    {
        var items = _basketService.GetBasketLines(UserSession.CurrentUser.Id, out decimal totalPrice);
        for (int i = 0; i < items.Count; i++)
        {
            System.Console.WriteLine($"[{i + 1}] {items[i]}");
        }
        Console.WriteLine("------------------------------");
        Console.WriteLine($"Total Amount: €{totalPrice:N2}");
    }

    public static void ShowBasket()
    {
        Console.Clear();
        if (UserSession.CurrentUser == null)
        {
            Console.WriteLine("⚠️ You must be logged in order for you to have a basket!");
            return;
        }

        var items = _basketService.GetBasketLines(UserSession.CurrentUser.Id, out decimal totalPrice);

        Console.WriteLine("--- 🛒 Your Shopping Cart ---");
        if (items.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
        }
        else
        {
            printBasket();
            Console.WriteLine("\n1. 💳 Pay Now (Checkout)");
            System.Console.WriteLine("2. Modify basket");
        }
        Console.WriteLine("0. Back");

        var choice = Console.ReadLine();

        switch(choice)
        {
            case "1":
                if (items.Count > 0)
                {
                    Console.Write("Confirm payment? (y/n): ");
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        if (_basketService.CheckoutWithTransaction(UserSession.CurrentUser.Id))
                        {
                            Console.WriteLine("\n✅ Payment successful! Thank you for your purchase.");
                            Console.WriteLine("press enter to return to the customer menu.");
                        }
                        else
                        {
                            Console.WriteLine("\n❌ Payment failed. Please try again.");
                            Console.WriteLine("press enter to return to the customer menu.");
                        }
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                Console.Clear();
                break;
            case "2":
                Console.Clear();
                printBasket();
                System.Console.WriteLine("\n1. remove item");
                System.Console.WriteLine("2. modify ammount of a item to purchase");
                System.Console.WriteLine("0. return");
                switch(Console.ReadLine())
                {
                    case "1":
                        System.Console.WriteLine("Which item would you like to delete?");
                        string optionItem = Console.ReadLine();
                        if (int.TryParse(optionItem, out int optionItemInt))
                        {
                            if ((optionItemInt - 1) >= 0 && (optionItemInt - 1) < items.Count)
                            {
                            
                                if (_basketService.RemoveFromBasket(UserSession.CurrentUser.Id, items[optionItemInt-1].ProductId))
                                {
                                    System.Console.WriteLine("Succesfully deleted item press any key to continue.");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    System.Console.WriteLine("Failed to delete item press any key to continue");
                                    Console.ReadKey();
                                }
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("Incorrect input");
                        }
                        break;
                    case "2":
                        break;
                    case "0":
                        break;

                }
                Console.Clear();
                break;
            case "0":
                Console.Clear();
                break;
        }
    }

    public static void PrintAll()
    {
        var products = _productService.GetAllProducts();
        Console.WriteLine("\n---- Product List ----");
        Console.WriteLine($"ID ---------- Name -------------------- Price");
        foreach (var p in products)
        {
            if (p != null)
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