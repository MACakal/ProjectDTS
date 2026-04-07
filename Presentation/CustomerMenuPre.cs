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
            BreadcrumbManager.Render();

            Console.WriteLine("\nCustomer Menu");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Make an Order");
            Console.WriteLine("3. View past Orders");
            Console.WriteLine("4. Filter Products");
            Console.WriteLine("5. View products ranking");
            System.Console.WriteLine("6. View account information");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                BreadcrumbManager.Push("View Products");
                Console.Clear();
                BreadcrumbManager.Render();
                _viewProductPres.Viewproducts();
                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
            if(choice == "2")
            {
                BreadcrumbManager.Push("Order");
                Console.Clear();
                BreadcrumbManager.Render();
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
            if(choice == "3")
            {
                BreadcrumbManager.Push("Past Orders");
                Console.Clear();
                BreadcrumbManager.Render();
                if(UserSession.CurrentUser == null)
                {
                    Console.WriteLine("⚠️ You must be logged in to view past order!");
                    Console.WriteLine("\nPress any key to return to the customer menu.");
                    Console.ReadLine();
                }
                else
                {
                    PastOrders.ShowPastOrders();
                    Console.WriteLine("\nPress any key to return to the customer menu.");
                    Console.ReadLine();
                }
            }
             if (choice == "4")
            {
                BreadcrumbManager.Push("Filter Products");
                Console.Clear();
                BreadcrumbManager.Render();
                _filterMenu.Show();

                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
            if (choice == "5")
            {
                BreadcrumbManager.Push("Products Ranking");
                Console.Clear();
                BreadcrumbManager.Render();
                ProductsRanking.Start();

                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
            if (choice == "6")
            {
                BreadcrumbManager.Push("Account Information");
                Console.Clear();
                BreadcrumbManager.Render();
                _AccountPre.AccountInformation(user);

            }
            if (UserSession.CurrentUser == null)
            {
                return;
            }
            if (choice == "0") break;
        }
    }
    
}




