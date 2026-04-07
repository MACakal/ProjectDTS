namespace ProjectDTS;

using System.Windows;

public class ViewProductPres
{
    private ProductService _productService;

    public ViewProductPres(ProductService productService)
    {
        _productService = productService;
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
        Console.WriteLine($"{"ID",-5} {"Name",-30} {"Category",-20} {"Price",10}");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(new string('-', 70));


        foreach (var p in items)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.Write($"{p.Id,-5}");  // { p.Name,-20} { p.Category,-20} { p.Price,10}€");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{p.Name,-30}");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{p.Category,-20}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"€{p.Price,10}");
            Console.ResetColor();
        }
    }
}