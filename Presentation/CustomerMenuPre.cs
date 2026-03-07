namespace ProjectDTS;


public class CustomerMenuPre
{

    private ViewProductPres _viewProductPres;
    private FilterMenu _filterMenu;

    public CustomerMenuPre(ViewProductPres viewProductPres, FilterMenu filterMenu) //ProductService productService, 
    {
        _viewProductPres = viewProductPres;
        _filterMenu = filterMenu;
    }

    public void CustomerShow()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nCustomer Menu");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Make an Order");
            Console.WriteLine("3. Filter Products");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.Clear();
                _viewProductPres.Viewproducts();

                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
            if (choice == "3")
            {
                _filterMenu.Show();

            }
            if (choice == "0") break;
        }
    }
}




