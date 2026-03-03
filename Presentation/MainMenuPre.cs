namespace ProjectDTS;

public class MainMenuPre
{
    private CustomerMenuPre _customerMenuPre;
    private AdminMenuPres _adminMenuPres;
    public MainMenuPre(CustomerMenuPre customerMenuPre, AdminMenuPres adminMenuPres)
    {
        _customerMenuPre = customerMenuPre;
        _adminMenuPres = adminMenuPres;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\nMain Menu");
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Admin");
            Console.WriteLine("0. Exit");
            var choice = Console.ReadLine();

            if (choice == "0")
            {
                Console.Clear();
                break;
            }

            if (choice == "1")
            {

                Console.Clear();
                _customerMenuPre.CustomerShow();
            }

            if (choice == "2")
            {
                Console.Clear();
                _adminMenuPres.ShowAdminMenu();

            }

        }
    }
}