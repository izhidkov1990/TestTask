using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

class Program
{
    private static DatabaseManager _dbManager;

    static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");
        _dbManager = new DatabaseManager(connectionString);

        try
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("1. Добавить нового сотрудника \n2. Посмотреть всех сотрудников\n3. Обновить информацию о сотруднике\n4. Удалить сотрудника\n5. Выйти из приложения");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddEmployee();
                        break;
                    case "2":
                        ViewAllEmployees();
                        break;
                    case "3":
                        UpdateEmployee();
                        break;
                    case "4":
                        DeleteEmployee();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
            }
        }
        finally
        {
            _dbManager.Dispose();
        }
    }

    //CRUD
    private static void AddEmployee()
    {
        var firstName = Prompt("Введите имя:");
        var lastName = Prompt("Введите фамилию:");
        var email = Prompt("Введите e-mail:");
        var dob = Prompt("Введите дату рождения (dd.MM.yyyy):");
        var salary = Prompt("Введите зарплату:");

        if (!IsValidName(firstName) || !IsValidName(lastName))
        {
            Console.WriteLine("Имя или фамилия содержат недопустимые символы.");
            return;
        }
        if (!IsValidEmail(email))
        {
            Console.WriteLine("Некорректный адрес электронной почты.");
            return;
        }
        if (!IsValidDate(dob))
        {
            Console.WriteLine("Некорректная дата рождения.");
            return;
        }
        var parsedSalary = ParseSalary(salary);
        if (!parsedSalary.HasValue)
        {
            Console.WriteLine("Некорректная сумма зарплаты. Введите число с не более чем двумя знаками после десятичной точки.");
            return;
        }

        var employee = new Employee
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = DateTime.ParseExact(dob, "dd.MM.yyyy", CultureInfo.InvariantCulture),
            Salary = parsedSalary.Value
        };

        _dbManager.AddEmployee(employee);
        Console.WriteLine("Сотрудник успешно добавлен.");
    }

    private static void ViewAllEmployees()
    {
        var employees = _dbManager.GetAllEmployees();
        foreach (var emp in employees)
        {
            Console.WriteLine($"ID: {emp.EmployeeID}, Имя: {emp.FirstName} {emp.LastName}, Email: {emp.Email}, Дата рождения: {emp.DateOfBirth.ToShortDateString()}, Зарплата: {emp.Salary}");
        }
    }

    private static void UpdateEmployee()
    {
        int employeeId = int.Parse(Prompt("Введите ID сотрудника:"));
        var firstName = Prompt("Введите новое имя:");
        var lastName = Prompt("Введите новую фамилию:");
        var email = Prompt("Введите новый Email:");
        var dob = Prompt("Введите новую дату рождения (dd.MM.yyyy):");
        var salary = Prompt("Введите зарплату:");

        if (!IsValidName(firstName) || !IsValidName(lastName))
        {
            Console.WriteLine("Имя или фамилия содержат недопустимые символы.");
            return;
        }
        if (!IsValidEmail(email))
        {
            Console.WriteLine("Некорректный адрес электронной почты.");
            return;
        }
        if (!IsValidDate(dob))
        {
            Console.WriteLine("Некорректная дата рождения.");
            return;
        }
        var parsedSalary = ParseSalary(salary);
        if (!parsedSalary.HasValue)
        {
            Console.WriteLine("Некорректная сумма зарплаты. Введите число с не более чем двумя знаками после десятичной точки.");
            return;
        }

        var employee = new Employee
        {
            EmployeeID = employeeId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = DateTime.ParseExact(dob, "dd.MM.yyyy", CultureInfo.InvariantCulture),
            Salary = parsedSalary.Value
        };

        _dbManager.UpdateEmployee(employee);
        Console.WriteLine("Данные о сотруднике успешно обновлены!");
    }

    private static void DeleteEmployee()
    {
        int employeeId = int.Parse(Prompt("Введите ID сотрудника для удаления:"));
        _dbManager.DeleteEmployee(employeeId);
        Console.WriteLine("Сотрудник успешно удален!");
    }

    private static string Prompt(string message)
    {
        Console.WriteLine(message);
        return Console.ReadLine();
    }


    //Check
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidDate(string date)
    {
        return DateTime.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    private static decimal? ParseSalary(string salary)
    {
        if (Regex.IsMatch(salary, @"^\d+(\.\d{1,2})?$"))
        {
            return decimal.Parse(salary, CultureInfo.InvariantCulture);
        }
        return null;
    }

    private static bool IsValidName(string name)
    {
        return Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯёЁ\-]+$");
    }

}
