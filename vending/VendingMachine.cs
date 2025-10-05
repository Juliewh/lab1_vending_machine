using System.Collections.Generic;
using System.Linq;

public class VendingMachine
{
    private Dictionary<int, Product> _products;

    private Dictionary<decimal, int> _currentBalance;

    private Dictionary<decimal, int> _machineBalance;

    public VendingMachine()
    {
        _products = new Dictionary<int, Product>();
        _currentBalance = new Dictionary<decimal, int>();
        _machineBalance = new Dictionary<decimal, int>();

        decimal[] acceptedCoins = new decimal[] { 1.00m, 2.00m, 5.00m, 10.00m, 50.00m, 100.00m, 200.00m, 500.00m };
        foreach (var coin in acceptedCoins)
        {
            _machineBalance.Add(coin, 0);
            _currentBalance.Add(coin, 0);
        }

        AddInitialMoney();
    }

    public void AddProduct(int id, Product product)
    {
        if (_products.ContainsKey(id))
        {
            _products[id].Quantity += product.Quantity;
        }
        else
        {
            _products.Add(id, product);
        }
    }

    public Dictionary<int, Product> GetProducts()
    {
        return new Dictionary<int, Product>(_products);
    }

    public void InsertCoin(decimal coin)
    {
        if (_machineBalance.ContainsKey(coin))
        {
            _currentBalance[coin]++;
        }
        else
        {
            throw new ArgumentException("Автомат не принимает монеты такого номинала.");
        }
    }

    public decimal GetCurrentBalance()
    {
        return _currentBalance.Sum(coin => coin.Key * coin.Value);
    }

    public string PurchaseProduct(int productId)
    {
        if (!_products.ContainsKey(productId))
            return "Ошибка: Товар не найден.";

        Product product = _products[productId];

        if (product.Quantity <= 0)
            return "Ошибка: Товар закончился.";

        decimal currentBalance = GetCurrentBalance();

        if (currentBalance < product.Price)
            return $"Ошибка: Внесено недостаточно средств. Внесено: {currentBalance:C}, требуется: {product.Price:C}.";

        decimal change = currentBalance - product.Price;

        if (!CanGiveChange(change))
        {
            return "Ошибка: В автомате недостаточно средств для выдачи сдачи. Операция отменена.";
        }

        product.Quantity--;

        foreach (var coin in _currentBalance)
        {
            _machineBalance[coin.Key] += coin.Value;
        }
        foreach (var key in _currentBalance.Keys.ToList())
        {
            _currentBalance[key] = 0;
        }

        string changeMessage = GiveChange(change);

        return $"Вы купили {product.Name}!{changeMessage}";
    }

    private bool CanGiveChange(decimal amount)
    {
        decimal totalInMachine = _machineBalance.Sum(coin => coin.Key * coin.Value);
        return totalInMachine >= amount;
    }

    private string GiveChange(decimal amount)
    {
        if (amount == 0)
            return "";

        string result = "\nВаша сдача: ";
        var changeCoins = new Dictionary<decimal, int>();
        decimal remainingChange = amount;

        foreach (var coin in _machineBalance.OrderByDescending(c => c.Key))
        {
            if (remainingChange <= 0) break;

            if (coin.Value > 0 && coin.Key <= remainingChange)
            {
                int numberOfCoins = (int)(remainingChange / coin.Key);
                numberOfCoins = Math.Min(numberOfCoins, coin.Value); 

                if (numberOfCoins > 0)
                {
                    changeCoins.Add(coin.Key, numberOfCoins);
                    remainingChange -= coin.Key * numberOfCoins;
                    remainingChange = Math.Round(remainingChange, 2); 
                    _machineBalance[coin.Key] -= numberOfCoins; 
                }
            }
        }

        if (changeCoins.Count > 0)
        {
            result += string.Join(", ", changeCoins.Select(c => $"{c.Value} x {c.Key:C}"));
        }
        else
        {
            result = "\nНе удалось выдать сдачу.";
        }

        return result;
    }

    public string AbortTransaction()
    {
        if (GetCurrentBalance() == 0)
            return "Нет средств для возврата.";

        string returnedCoins = string.Join(", ", _currentBalance
            .Where(coin => coin.Value > 0)
            .Select(coin => $"{coin.Value} x {coin.Key:C}"));

        foreach (var key in _currentBalance.Keys.ToList())
        {
            _currentBalance[key] = 0;
        }

        return $"Операция отменена. Возвращено: {returnedCoins}";
    }

    public decimal CollectMoney()
    {
        decimal total = _machineBalance.Sum(coin => coin.Key * coin.Value);
        foreach (var key in _machineBalance.Keys.ToList())
        {
            _machineBalance[key] = 0;
        }
        return total;
    }

    private void AddInitialMoney()
    {
        _machineBalance[1.00m] = 20;   
        _machineBalance[2.00m] = 20;   
        _machineBalance[5.00m] = 10;   
        _machineBalance[10.00m] = 10;  
        _machineBalance[50.00m] = 4;   
        _machineBalance[100.00m] = 3;  

    }
}
