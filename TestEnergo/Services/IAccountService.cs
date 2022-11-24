using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TestEnergo.Constants;
using TestEnergo.ViewModels.EmployeeViewModels;

namespace TestEnergo.Services
{
    public interface IAccountService
    {
        public EmployeeViewModel? Login(EmployeeLoginViewModel employee);
    }

    public class AccountService : IAccountService
    {
        private readonly MyConnection _myConnection;

        public AccountService(IOptions<MyConnection> myOptions)
        {
            _myConnection = myOptions.Value;
        }

        public EmployeeViewModel? Login(EmployeeLoginViewModel employee)
        {
            var sqlExpression = $@"SELECT Employees._id, Employees.Login, Employees.Password, Employees.Name, Employees.Role, Departments.Name
                                    FROM Employees
                                    LEFT JOIN Departments ON Departments._id = Employees.DepartmentID
                                    WHERE Login = '{employee.Login}' AND Password = '{employee.Password}'";
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        
                        return new EmployeeViewModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Name = reader.GetString(3),
                            Role = reader.GetString(4),
                            DepartmentName = reader.IsDBNull(5) ? null : reader.GetString(5) 
                        };
                    }
                    return null;
                }
            }
        }
    }
}
