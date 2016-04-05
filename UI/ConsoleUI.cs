using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eshop.Data;
using System.Data;
using System.Data.SqlClient;

namespace Eshop.UI
{
    public class ConsoleUI
    {
        private DbManager _database;
        private Pirkejas _logedBuyer;

        public ConsoleUI(DbManager database)
        {
            _database = database;
        }

        public void RunUI()
        {
            bool inLoop = true;
            while (inLoop)
            {
                ShowLoginMenu();
                switch (ParseSelection(Console.ReadLine()))
                {
                    case (1):
                        Console.Write("Iveskite savo el. pasto edresa:");
                        string userEmail = Console.ReadLine();
                        _logedBuyer = _database.Authentification(userEmail);
                        if (_logedBuyer != null)
                            inLoop = false;
                        else
                            Console.WriteLine("Bandykite dar karta");
                        break;
                    case (2):
                        bool registrationLoop = true;
                        string firstName;
                        string lastName;
                        string email;
                        string telNum;
                        string city;
                        string street;
                        string houseNum;
                        string apartmentNum;
                        while (registrationLoop)
                        {
                            Console.Write("Vardas:");
                            firstName = Console.ReadLine();
                            Console.Write("Pavarde:");
                            lastName = Console.ReadLine();
                            Console.Write("El.pastas:");
                            email = Console.ReadLine();
                            Console.Write("Tel. Nr:");
                            telNum = Console.ReadLine();
                            Console.Write("Miestas:");
                            city = Console.ReadLine();
                            Console.Write("Gatve:");
                            street = Console.ReadLine();
                            Console.Write("Namas:");
                            houseNum = Console.ReadLine();
                            Console.Write("Butas:");
                            apartmentNum = Console.ReadLine();

                            if (!string.IsNullOrEmpty(firstName) &&
                                !string.IsNullOrEmpty(lastName) &&
                                !string.IsNullOrEmpty(email) &&
                                !string.IsNullOrEmpty(telNum) &&
                                !string.IsNullOrEmpty(city) &&
                                !string.IsNullOrEmpty(street) &&
                                !string.IsNullOrEmpty(houseNum) &&
                                !string.IsNullOrEmpty(apartmentNum))

                            {
                                try {
                                    _database.RegisterUser(firstName, lastName, email, telNum, city, street, houseNum, apartmentNum);
                                    Console.WriteLine("Registracija sekmingai ivyko");
                                    _logedBuyer = _database.Authentification(email);
                                    registrationLoop = false;
                                    inLoop = false;
                                }
                                catch(SqlException e)
                                {
                                    Console.WriteLine("Registracija nepavyko, bandykite dar karta");
                                }
                            }
                        }
                        break;
                    case (3):
                        inLoop = false;
                        break;
                    default:
                        break;
                }
            }
            if (_logedBuyer != null)
                RunUserMenu();
        }

        private void RunUserMenu()
        {
            bool inLoop = true;
            while (inLoop)
            {
                ShowMenu();
                switch (ParseSelection(Console.ReadLine()))
                {
                    case (1):
                        GetAllUserOrders();
                        break;
                    case (2):
                        GetListOfProducts();
                        break;
                    case (3):
                        MakeOrderMenu();
                        break;
                    case (4):
                        inLoop = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void MakeOrderMenu()
        {
            Uzsakymas order = _database.CreateOrder(_logedBuyer);
            GetListOfProducts();
            int productId;
            int quantity;
            bool inLoop = true;
            while (inLoop)
            {
                Console.Write("Iveskite prekes ID:");
                productId = ParseSelection(Console.ReadLine());
                Console.Write("Iveskite norima kieki:");
                quantity = ParseSelection(Console.ReadLine());
                if (!_database.AddProductToOrder(order, productId, quantity))
                    Console.WriteLine("Neteisingas pasirinkimas");
                Console.WriteLine("*************************");
                Console.WriteLine("1-Prideti dar viena preke\n" +
                                  "2-Baigti uzsakyma\n" +
                                  "3-Pasalinti uzsakyma");
                Console.WriteLine("*************************");
                switch (ParseSelection(Console.ReadLine()))
                {
                    case (1):
                        ShowOrderInfo(order.Nr);
                        break;
                    case (2):
                        decimal sum = _database.GetOrderSum(order);
                        Console.WriteLine("Uzsakymo suma:" + sum);
                        inLoop = false;
                        _database.EndOrder(order);
                        break;
                    case (3):
                        _database.RemoveOrder(order);
                        inLoop = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void GetListOfProducts()
        {
            string line = new string('-', 121);
            var productsInfo = _database.GetProductsInfo();
            foreach (var category in productsInfo)
            {
                Console.WriteLine("\n" + category.Key + ":");

                string result = String.Format("|{0,-5}|{1,-40}|{2,-15}|{3,-9}|{4,-8}|{5,-17}|{6,-9}|{7,-9}|",
                                                "ID",
                                                "Pavadinimas",
                                                "Gamintojas",
                                                "Kaina",
                                                "Nuolaida",
                                                "Kaina su nuolaida",
                                                "Garantija",
                                                "Liko vnt.");
                Console.WriteLine(result);
                Console.WriteLine(line);
                foreach (var product in category)
                {
                    string resultProduct = String.Format("|{0,-5}|{1,-40}|{2,-15}|{3,-9}|{4,-8}|{5,-17}|{6,-9}|{7,-9}|", product.ID, product.Pavadinimas,
                                      product.Gamintojas, product.Kaina,
                                      product.Nuolaida, product.KainaSuNuolaida,
                                      product.Garantija, product.Liko);
                    Console.WriteLine(resultProduct);
                }
            }
        }

        private void GetAllUserOrders()
        {
            DataTable userOrder = _database.GetUserOders(_logedBuyer);
            Console.WriteLine(
                String.Format("|{0,-11}|{1,-17}|{2,-17}|{3,-17}|{4,-9}|",
                                                "Uzsakymo ID",
                                                "Priemimo data",
                                                "Sandeliavimo data",
                                                "Atidavimo data",
                                                "Moketi")
                );
            Console.WriteLine(new String('-', 77));
            foreach (DataRow row in userOrder.Rows)
            {
                Console.WriteLine(
                    String.Format("|{0,-11}|{1,-17:d}|{2,-17:d}|{3,-17:d}|{4,-9}|",
                                    row[0],
                                    row[1],
                                    row[2],
                                    row[3],
                                    row[4])
                );
            }
            Console.WriteLine("*************************");
            Console.WriteLine("1-Detaliau perziureti uzsakyma\n" +
                              "2-Iseiti i pagrindini meniu");
            Console.WriteLine("*************************");
            switch (ParseSelection(Console.ReadLine()))
            {
                case (1):
                    Console.Write("Iveskite uzsakymo numeri:");
                    int id = ParseSelection(Console.ReadLine());
                    ShowOrderInfo(id);
                    break;
                default:
                    break;
            }
        }

        private void ShowOrderInfo(int id)
        {
            DataTable orderInfo = _database.GetOrderInfo(_logedBuyer, id);
            if (orderInfo != null)
            {
                Console.WriteLine(String.Format("|{0,-40}|{1,-15}|{2,-5}|{3,-9}|",
                                        "Pavadinimas",
                                        "Gamintojas",
                                        "Kiekis",
                                        "Kaina"));
                Console.WriteLine(new String('-', 74));
                foreach (DataRow row in orderInfo.Rows)
                {
                    Console.WriteLine(String.Format("|{0,-40}|{1,-15}|{2,-6}|{3,-9}|",
                                        row[0],
                                        row[1],
                                        row[2],
                                        row[3]));
                }
            }
        }

        private void ShowMenu()
        {
            Console.WriteLine("*************************");
            Console.WriteLine("1-Atliktu uzsakymu sarasas\n" +
                              "2-Prekiu sarasas\n" +
                              "3-Atlikti uzsakyma\n" +
                              "4-Iseiti");
            Console.WriteLine("*************************");
        }

        private void ShowLoginMenu()
        {
            Console.WriteLine("*************************");
            Console.WriteLine("1-Prisijungti\n" +
                              "2-Registruotis\n" +
                              "3-Iseiti");
            Console.WriteLine("*************************");
        }

        private int ParseSelection(String selection)
        {
            while (true)
            {
                try
                {
                    if (!String.IsNullOrEmpty(selection) || !selection.Equals("\n"))
                        return int.Parse(selection);
                    else
                    {
                        Console.WriteLine("Dar kartą įveskite pasirinktą skaičių");
                        selection = Console.ReadLine();
                    }
                }
                catch
                {
                    Console.WriteLine("Dar kartą įveskite pasirinktą skaičių");
                    selection = Console.ReadLine();
                }
            }
        }


    }
}
