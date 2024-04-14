using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class DatabaseManager : IDisposable
{
    private readonly string _connectionString;
    private SqlConnection _connection;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    private void EnsureConnected()
    {
        if (_connection == null)
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }
        else if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public void AddEmployee(Employee employee)
    {
        try
        {
            EnsureConnected();
            var query = "INSERT INTO Employees (FirstName, LastName, Email, DateOfBirth, Salary) VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @Salary)";
            using (var command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                command.Parameters.AddWithValue("@Email", employee.Email);
                command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Ошибка при добавлении сотрудника: " + ex.Message);
        }
    }

    public List<Employee> GetAllEmployees()
    {
        var employees = new List<Employee>();
        try
        {
            EnsureConnected();
            var query = "SELECT * FROM Employees";
            using (var command = new SqlCommand(query, _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            EmployeeID = (int)reader["EmployeeID"],
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            DateOfBirth = (DateTime)reader["DateOfBirth"],
                            Salary = (decimal)reader["Salary"]
                        });
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Ошибка при получении списка сотрудников: " + ex.Message);
        }
        return employees;
    }

    public void UpdateEmployee(Employee employee)
    {
        try
        {
            EnsureConnected();
            var query = "UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, Email = @Email, DateOfBirth = @DateOfBirth, Salary = @Salary WHERE EmployeeID = @EmployeeID";
            using (var command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                command.Parameters.AddWithValue("@Email", employee.Email);
                command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                command.Parameters.AddWithValue("@Salary", employee.Salary);
                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Ошибка при обнолении информации по сотруднику: " + ex.Message);
        }
    }

    public void DeleteEmployee(int employeeId)
    {
        try
        {
            EnsureConnected();
            var query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
            using (var command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                command.ExecuteNonQuery();
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Ошибка при удалении сотрудника: " + ex.Message);
        }
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
