namespace ProjectDTS;

using System.Windows;

public class ViewProductPres
{
    private ProductService _productService;
    private RatingService _ratingService;

    public ViewProductPres(ProductService productService, RatingService ratingService = null)
    {
        _productService = productService;
        _ratingService = ratingService ?? new RatingService(new DatabaseService());
    }
    public void Viewproducts()
    {

        Console.Clear();
        var products = _productService.GetAllProducts();

        Viewproducts(products);
    }

    public void Viewproducts(IEnumerable<Product> items, bool showBasket = true)
    {


        DisplayProducts(items);
        if (showBasket)
            BasketMenu.WhatToDo();
    }



    public void DisplayProducts(IEnumerable<Product> items)
    {
        ConsoleColor[] colors = {
    ConsoleColor.Red,
    ConsoleColor.Yellow,
    ConsoleColor.Green,
    ConsoleColor.Cyan,
    ConsoleColor.Blue,
    ConsoleColor.Magenta
    };

        string text = "\n            --------- Product List ---------";

        for (int i = 0; i < text.Length; i++)
        {
            Console.ForegroundColor = colors[i % colors.Length];
            Console.Write(text[i]);
        }

        Console.WriteLine();
        Console.ResetColor();

        if (!items.Any())
        {
            Console.WriteLine("There are no products to show.");
            return;
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{"ID",-5} {"Name",-30} {"Category",-20} {"Price",10} {"Rating",10}");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(new string('-', 90));

        foreach (var p in items)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.Write($"{p.Id,-5}");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{p.Name,-30}");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{p.Category,-20}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"€{p.Price,10}");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            string ratingDisplay = p.RatingCount > 0 
                ? $"★ {p.AverageRating:F1}/5 ({p.RatingCount})" 
                : "Not rated";
            Console.WriteLine($"{ratingDisplay,10}");
            Console.ResetColor();
        }
    }

    public void DisplayProductWithRatings(Product product)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n========== {product.Name} ==========");
        Console.ResetColor();

        Console.WriteLine($"Category: {product.Category}");
        Console.WriteLine($"Price: €{product.Price}");
        Console.WriteLine($"Description: {product.Description}");
        Console.WriteLine($"Rarity: {product.Rarity}");

        Console.ForegroundColor = ConsoleColor.Yellow;
        if (product.RatingCount > 0)
        {
            Console.WriteLine($"\nAverage Rating: ★ {product.AverageRating:F1}/5 ({product.RatingCount} ratings)");
        }
        else
        {
            Console.WriteLine("\nNo ratings yet");
        }
        Console.ResetColor();

        var ratings = _ratingService.GetProductRatings(product.Id);
        if (ratings.Any())
        {
            Console.WriteLine("\n--- Recent Reviews ---");
            foreach (var rating in ratings.Take(5))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"★ {rating.RatingValue}/5 | ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{rating.CreatedAt:g}");
                if (!string.IsNullOrEmpty(rating.ReviewText))
                {
                    Console.WriteLine($"  \"{rating.ReviewText}\"");
                }
                Console.ResetColor();
            }
        }
    }

    public void RateProduct(Product product, int userId)
    {
        Console.Clear();
        Console.WriteLine($"Rate: {product.Name}");
        Console.WriteLine("Rating (1-5 stars):");
        
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
        {
            Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
            return;
        }

        Console.WriteLine("Review (optional, press Enter to skip):");
        string review = Console.ReadLine();

        try
        {
            _ratingService.AddOrUpdateRating(product.Id, userId, rating, string.IsNullOrWhiteSpace(review) ? null : review);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Rating submitted successfully!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error submitting rating: {ex.Message}");
            Console.ResetColor();
        }
    }
}