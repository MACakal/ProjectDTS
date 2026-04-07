namespace ProjectDTS;

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
        Console.Clear();
        Console.WriteLine("\n---- Product List ----");

        if (!items.Any())
        {
            Console.WriteLine("There are no products to show.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Name",-20} {"Category",-20} {"Price",10}");
        Console.WriteLine(new string('-', 60));

        foreach (var p in items)
        {
            Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Category,-20} {p.Price,10}€");
        }
    }
}