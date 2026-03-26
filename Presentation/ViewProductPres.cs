namespace ProjectDTS;

public class ViewProductPres
{
    private FilterMenu _filterMenu;
    private ProductService _productService;

    public ViewProductPres(ProductService productService, FilterMenu filterMenu)
    {
        _productService = productService;
        _filterMenu = filterMenu;
    }
    public void Viewproducts()
    {

        Console.Clear();
        var products = _productService.GetAllProducts();
        Console.WriteLine("\n---- Product List ----");
        if (products.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("There are no products to show.");

        }
        else
        {

            Console.WriteLine($"{"ID",-5} {"Name",-20} {"Category",-20} {"Price",10}");
            Console.WriteLine(new string('-', 60));
            foreach (var p in products.OrderBy(products => products.Id))
            {

                Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Category,-20} {p.Price,10}€");

            }
            BasketMenu.WhatToDo();
        }
    }

    public void Viewproducts(List<Product> items)
    {

        Console.Clear();
        Console.WriteLine("\n---- Product List ----");
        if (items.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("There are no products to show.");

        }
        else
        {

            Console.WriteLine($"{"ID",-5} {"Name",-20} {"Category",-20} {"Price",10}");
            Console.WriteLine(new string('-', 60));
            foreach (var p in items)
            {

                Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Category,-20} {p.Price,10}€");

            }
            BasketMenu.WhatToDo();
        }
    }

}