using Npgsql;
using NpgsqlTypes;
namespace ProjectDTS;

public class BasketMenu
{

    private static ProductService _productService;
    private static FilterMenu _filterMenu;
    private static BasketService _basketService;
    private static SortingMenu _sortingMenu;
    private static RatingService _ratingService;
    private static ViewProductPres _viewProductPres;

    public BasketMenu(ProductService productService, FilterMenu filterMenu, BasketService basketService, SortingMenu sortingMenu, RatingService ratingService)
    {
        _productService = productService;
        _filterMenu = filterMenu;
        _basketService = basketService;
        _sortingMenu = sortingMenu;
        _ratingService = ratingService;
    }
    public static void WhatToDo()
    {
        while (true) // Zorgt dat je in dit menu blijft tot je '0' kiest
        {
            System.Console.WriteLine("\nWhat do you want to do?");
            System.Console.WriteLine("1. View product");
            System.Console.WriteLine("2. Add product to basket");
            System.Console.WriteLine("3. Filter products");
            System.Console.WriteLine("4. View Basket");
            System.Console.WriteLine("5. Sort");
            System.Console.WriteLine("6. Rate product");
            System.Console.WriteLine("0. Back");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Clear();
                    ViewItem();
                    break;
                case "2":
                    Console.Clear();
                    AddToBasketPrint();
                    break;
                case "3":
                    Console.Clear();
                    _filterMenu.Show();
                    break;
                case "4":
                    Console.Clear();
                    ShowBasket();
                    break;
                case "5":
                    Console.Clear();
                    _sortingMenu.Show();
                    break;
                case "6":
                    Console.Clear();
                    RateProductMenu();
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

    public static void ViewItem()
    {
        //BreadcrumbManager.Push("Product Details");
        //BreadcrumbManager.Render();
        if (UserSession.CurrentUser == null)
        {
            Console.WriteLine("⚠️ You must be logged in to view products!");
            return;
        }

        PrintAll();
        Console.WriteLine();
        Console.WriteLine("--- Pick product to view ---");
        Console.Write("Enter product ID: ");

        if (!int.TryParse(Console.ReadLine(), out int productId))
        {
            Console.WriteLine("❌ Invalid product ID!");
            return;
            //BreadcrumbManager.Pop();
        }

        var product = _productService.GetById(productId);

        if (product == null)
        {
            Console.WriteLine("❌ Product not found!");
            return;
            //BreadcrumbManager.Pop();
        }

        System.Console.WriteLine("\n================ Product Details ================");
        System.Console.WriteLine($"ID: {product.Id}");
        System.Console.WriteLine($"Name: {product.Name}");
        System.Console.WriteLine($"Description: {product.Description}");
        
        if (product.RatingCount > 0)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine($"Rating: ★ {product.AverageRating:F1}/5 ({product.RatingCount} ratings)");
            System.Console.ResetColor();
        }
        else
        {
            System.Console.WriteLine("Rating: Not rated yet");
        }
        
        System.Console.WriteLine($"Category: {product.Category}");
        System.Console.WriteLine($"Price: ${product.Price:F2}");
        System.Console.WriteLine($"Rarity: {product.Rarity}");
        System.Console.WriteLine($"Views: {product.View_count}");
        System.Console.WriteLine($"Purchases: {product.Purchase_count}");
        System.Console.WriteLine("===============================================\n");
        System.Console.WriteLine("Press any key to continue.");
        System.Console.ReadKey();
        Console.Clear();
        //BreadcrumbManager.Pop();
    }

    public static void printBasket()
    {
        var items = _basketService.GetBasketLines(UserSession.CurrentUser.Id, out decimal totalPrice);
        Console.WriteLine("--- 🛒 Your Shopping Cart ---");
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
        //BreadcrumbManager.Push("Basket");
        //BreadcrumbManager.Render();
        if (UserSession.CurrentUser == null)
        {
            Console.WriteLine("⚠️ You must be logged in order for you to have a basket!");
            return;
        }

        var items = _basketService.GetBasketLines(UserSession.CurrentUser.Id, out decimal totalPrice);

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

        switch (choice)
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
                        //BreadcrumbManager.Pop();
                    }
                }
                Console.Clear();
                break;
            case "2":
                Console.Clear();
                //BreadcrumbManager.Push("Modify Basket");
                //BreadcrumbManager.Render();
                printBasket();
                System.Console.WriteLine("\n1. remove item");
                System.Console.WriteLine("2. modify ammount of a item to purchase");
                System.Console.WriteLine("0. return");
                switch (Console.ReadLine())
                {
                    case "1":
                        System.Console.WriteLine("Which item would you like to delete?");
                        string optionItem = Console.ReadLine();
                        if (int.TryParse(optionItem, out int optionItemInt))
                        {
                            if ((optionItemInt - 1) >= 0 && (optionItemInt - 1) < items.Count)
                            {

                                if (_basketService.RemoveFromBasket(UserSession.CurrentUser.Id, items[optionItemInt - 1].ProductId))
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
                        System.Console.WriteLine("Which item would you like to change quantity of?");
                        string optionItemModify = Console.ReadLine();
                        if (int.TryParse(optionItemModify, out int optionItemModifyInt))
                        {
                            if ((optionItemModifyInt - 1) >= 0 && (optionItemModifyInt - 1) < items.Count)
                            {
                                System.Console.WriteLine("What would you like to change the quantity to?");
                                if (int.TryParse(Console.ReadLine(), out int newQuantity))
                                {
                                    if (_basketService.ModifyQuantityBasket(UserSession.CurrentUser.Id, items[optionItemModifyInt - 1].ProductId, newQuantity))
                                    {
                                        System.Console.WriteLine("Succesfully updated item press any key to continue.");
                                        Console.ReadKey();
                                    }
                                    else
                                    {
                                        System.Console.WriteLine("Failed to updated item press any key to continue");
                                        Console.ReadKey();
                                    }
                                }

                            }
                        }
                        else
                        {
                            System.Console.WriteLine("Incorrect input");
                        }
                        break;
                    case "0":
                        //BreadcrumbManager.Pop();
                        break;

                }
                Console.Clear();
                //BreadcrumbManager.Pop();
                break;
            case "0":
                Console.Clear();
                //BreadcrumbManager.Pop();
                break;
        }
    }

    public static void PrintAll()
    {
        var products = _productService.GetAllProducts();
        ViewProductPres view = new(_productService, _ratingService);
        view.DisplayProducts(products);
    }

    public static void RateProductMenu()
    {
        if (UserSession.CurrentUser == null)
        {
            Console.WriteLine("⚠️ You must be logged in to rate products!");
            return;
        }

        PrintAll();
        Console.WriteLine();
        Console.WriteLine("--- Rate a Product ---");
        Console.Write("Enter product ID to rate: ");

        if (!int.TryParse(Console.ReadLine(), out int productId))
        {
            Console.WriteLine("❌ Invalid product ID!");
            return;
        }

        var product = _productService.GetById(productId);

        if (product == null)
        {
            Console.WriteLine("❌ Product not found!");
            return;
        }

        // Display product info and current ratings
        Console.WriteLine($"\n📦 {product.Name}");
        if (product.RatingCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Current Rating: ★ {product.AverageRating:F1}/5 ({product.RatingCount} ratings)");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("Current Rating: Not rated yet");
        }

        // Get user's existing rating if any
        var userRating = _ratingService.GetUserRatingForProduct(productId, UserSession.CurrentUser.Id);
        if (userRating != null)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Your previous rating: ★ {userRating.RatingValue}/5");
            Console.ResetColor();
        }

        Console.WriteLine("\n⭐ Rate this product (1-5 stars):");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
        {
            Console.WriteLine("❌ Invalid rating. Please enter a number between 1 and 5.");
            return;
        }

        Console.WriteLine("\n💬 Add a review (optional, press Enter to skip):");
        string review = Console.ReadLine();

        try
        {
            _ratingService.AddOrUpdateRating(productId, UserSession.CurrentUser.Id, rating, string.IsNullOrWhiteSpace(review) ? null : review);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Rating submitted successfully!");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error submitting rating: {ex.Message}");
            Console.ResetColor();
        }
    }
}