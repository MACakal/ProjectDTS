namespace ProjectDTS;


public class CustomerMenuPre
{
    private ViewProductPres _viewProductPres;
    // private ProductService _productService;
    public CustomerMenuPre(ViewProductPres viewProductPres) //ProductService productService, 
    {
        // _productService = productService;
        _viewProductPres = viewProductPres;
    }
    public void CustomerShow()
    {
        while (true)
        {

            Console.WriteLine("\nCustomer Menu");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Make an Order");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.Clear();
                _viewProductPres.Viewproducts();
                // var products = _productService.GetAllProducts();

                // Console.WriteLine("\n---- Product List ----");
                // if (products.Count == 0)
                // {
                //     Console.Clear();
                //     Console.WriteLine("There are no products to show.");

                // }
                // else
                // {

                //     Console.WriteLine($"{"ID",-5} {"Name",-20} {"Price",10}");
                //     Console.WriteLine(new string('-', 40));
                //     foreach (var p in products)
                //     {

                //         Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Price,10}€");
                //     }
                // }
            }
            if (choice == "0") break;
        }
    }



}
