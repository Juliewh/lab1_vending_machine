using System;
using System.Collections.Generic;

class Program
{
    static VendingMachine _vm = new VendingMachine();

    static void Main(string[] args)
    {
        InitializeProducts();

        bool isRunning = true;
        while (isRunning)
        {
            Console.Clear();
            ShowWelcomeScreen(); 

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": InsertCoins(); break;
                case "2": BuyProduct(); break;
                case "3": AbortTransaction(); break;
                case "9": AdminMode(); break;
                case "0": isRunning = false; break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    static void ShowWelcomeScreen()
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("===      ВЕНДИНГОВЫЙ АВТОМАТ          ===");
        Console.WriteLine("==========================================");
        Console.WriteLine();

        ShowProducts(); 
        
        Console.WriteLine();
        Console.WriteLine("Доступные дйствия:");
        Console.WriteLine("1 - Внести монеты");
        Console.WriteLine("2 - Выбрать товар для покупки");
        Console.WriteLine("3 - Вернуть деньги");
        Console.WriteLine("9 - Войти в административный режим");
        Console.WriteLine();

    }

    static void InitializeProducts()
    {
        _vm.AddProduct(1, new Product("Кола", 80m, 10));
        _vm.AddProduct(2, new Product("Чипсы", 100m, 8));
        _vm.AddProduct(3, new Product("Шоколад", 60m, 5));
        _vm.AddProduct(4, new Product("Вода", 35m, 15));
    }

    static void ShowProducts()
    {
        Console.WriteLine("\n--- ДОСТУПНЫЕ ТОВАРЫ ---");
        var products = _vm.GetProducts();
        if (products.Count == 0)
        {
            Console.WriteLine("Товаров нет.");
            return;
        }
        foreach (var item in products)
        {
            Console.WriteLine($"[{item.Key}] {item.Value}");
        }
        Console.WriteLine($"\nВнесенная сумма: {_vm.GetCurrentBalance():C}");
    }

    static void InsertCoins()
    {
        Console.WriteLine("Доступные номиналы: 1, 2, 5, 10, 50, 100, 200, 500");
        Console.Write("Введите номинал монеты: ");

        if (decimal.TryParse(Console.ReadLine(), out decimal coin))
        {
            try
            {
                _vm.InsertCoin(coin);
                Console.WriteLine($"Монета {coin:C} принята. Текущий баланс: {_vm.GetCurrentBalance():C}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            Console.WriteLine("Неверный формат.");
        }
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    static void BuyProduct()
    {
        Console.Write("Введите номер товара для покупки: ");
        if (int.TryParse(Console.ReadLine(), out int productId))
        {
            string result = _vm.PurchaseProduct(productId);
            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine("Неверный номер товара.");
        }
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    static void AbortTransaction()
    {
        string result = _vm.AbortTransaction();
        Console.WriteLine(result);
    }

    static void AdminMode()
    {
        Console.Write("Введите пароль администратора (по умолчанию 'admin'): ");
        string password = Console.ReadLine();
        if (password != "admin")
        {
            Console.WriteLine("Неверный пароль.");
            return;
        }

        bool inAdminMode = true;
        while (inAdminMode)
        {
            Console.Clear();
            Console.WriteLine("=== АДМИНИСТРАТИВНЫЙ РЕЖИМ ===");
            Console.WriteLine("1. Пополнить существующий товар");
            Console.WriteLine("2. Добавить новый товар"); 
            Console.WriteLine("3. Собрать деньги");
            Console.WriteLine("0. Назад");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Введите ID товара для пополнения: ");
                    int id = int.Parse(Console.ReadLine());
                    Console.Write("Введите количество для добавления: ");
                    int qty = int.Parse(Console.ReadLine());
                    _vm.AddProduct(id, new Product("", 0m, qty));
                    Console.WriteLine($"Товар {id} пополнен на {qty} единиц.");
                    break;

                case "2": 
                    Console.Write("Введите ID для нового товара: ");
                    int newId = int.Parse(Console.ReadLine());
                    Console.Write("Введите название товара: ");
                    string name = Console.ReadLine();
                    Console.Write("Введите цену товара: ");
                    decimal price = decimal.Parse(Console.ReadLine());
                    Console.Write("Введите начальное количество: ");
                    int newQty = int.Parse(Console.ReadLine());

                    _vm.AddProduct(newId, new Product(name, price, newQty));
                    Console.WriteLine($"Создан новый товар: {name} за {price:C}");
                    break;

                case "3": 
                    decimal collected = _vm.CollectMoney();
                    Console.WriteLine($"Собрано денег: {collected:C}");
                    break;

                case "0":
                    inAdminMode = false;
                    break;

                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}
