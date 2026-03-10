namespace ProjectDTS;


public class CustomerMenuPre
{

    private ViewProductPres _viewProductPres;
    private FilterMenu _filterMenu;
    private AccountPre _AccountPre;

    public CustomerMenuPre(ViewProductPres viewProductPres, FilterMenu filterMenu, AccountPre accountPre) //ProductService productService, 
    {
        _viewProductPres = viewProductPres;
        _filterMenu = filterMenu;
        _AccountPre = accountPre;
    }

    public void CustomerShow(User user)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nCustomer Menu");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Make an Order");
            Console.WriteLine("3. Filter Products");
            System.Console.WriteLine("4. View account information");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    _viewProductPres.Viewproducts();

                    Console.WriteLine("\nPress any key to return to the customer menu.");
                    Console.ReadLine();
                    break;
                case "3":
                    _filterMenu.Show();
                    break;
                case "4":
                    _AccountPre.AccountInformation(user);
                    break;
                case "0":
                    return;
            }
        }
    }
}




