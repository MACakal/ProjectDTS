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
            }
            if(choice == "2")
            {
                Console.Clear();
                if(UserSession.CurrentUser == null)
                {
                    Console.WriteLine("⚠️ You must be logged in to make an order!");
                    Console.WriteLine("\nPress any key to return to the customer menu.");
                    Console.ReadLine();
                }
                else
                {
                    BasketMenu.ShowBasket();
                }
            }
             if (choice == "3")
            {
                Console.Clear();
                _filterMenu.Show();

                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
             if (choice == "0") break;
            if (choice == "3")
            {
                _filterMenu.Show();

            }
            if (choice == "0") break;
        }
    }
}




