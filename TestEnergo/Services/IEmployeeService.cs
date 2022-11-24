using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TestEnergo.Constants;
using TestEnergo.ViewModels.EmployeeViewModels;

namespace TestEnergo.Services
{
    public interface IEmployeeService
    {
        public List<EmployeeViewModel> GetAll();
        public EmployeeViewModel? GetById(int id);
        public List<EmployeeViewModel> GetDepartmentEmployees(int departmentId);
        public List<EmployeeViewModel> Search(string? searchString);
        public EmployeeCreateViewModel? Create(EmployeeCreateViewModel employee);
        public EmployeeUpdateViewModel? Update(EmployeeUpdateViewModel employee);
        public int? Delete(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly MyConnection _myConnection;

        public EmployeeService(IOptions<MyConnection> myConnection)
        {
            _myConnection = myConnection.Value;
        }

        public List<EmployeeViewModel> GetAll()
        {
            var sqlExpression =
                $@"SELECT Employees._id, Employees.Login, Employees.Password, Employees.Name, Employees.Role, Departments.Name
                 FROM Employees
                 LEFT JOIN Departments ON Departments._id = Employees.DepartmentID";
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var employees = new List<EmployeeViewModel>();

                    if (!reader.HasRows) return employees;

                    while (reader.Read())
                    {
                        employees.Add(new EmployeeViewModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Name = reader.GetString(3),
                            Role = reader.GetString(4),
                            DepartmentName = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }

                    return employees;
                }
            }
        }

        public EmployeeViewModel? GetById(int id)
        {
            var sqlExpression =
                $@"SELECT Employees._id, Employees.Login, Employees.Password, Employees.Name, Employees.Role, Departments.Name 
                 FROM Employees
                 LEFT JOIN Departments ON Departments._id = Employees.DepartmentID
                 WHERE Employees._id = {id}";
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return null;

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
            }
        }

        public List<EmployeeViewModel> GetDepartmentEmployees(int departmentId)
        {
            var sqlExpression =
                $@"SELECT Employees._id, Employees.Login, Employees.Password, Employees.Name, Employees.Role, Departments.Name
                 FROM Employees
                 LEFT JOIN Departments ON Departments._id = Employees.DepartmentID
                 WHERE Employees.DepartmentID = {departmentId}";

            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var employees = new List<EmployeeViewModel>();

                    if (!reader.HasRows) return employees;

                    while (reader.Read())
                    {
                        employees.Add(new EmployeeViewModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Name = reader.GetString(3),
                            Role = reader.GetString(4),
                            DepartmentName = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }

                    return employees;
                }
            }
        }

        public List<EmployeeViewModel> Search(string? searchString)
        {
            var employees = new List<EmployeeViewModel>();
            if (searchString == null) return employees;
            
            var sqlExpression =
                $@"SELECT Employees._id, Employees.Login, Employees.Password, Employees.Name, Employees.Role, Departments.Name
                 FROM Employees
                 LEFT JOIN Departments ON Departments._id = Employees.DepartmentID
                 WHERE INSTR(Employees.Login, '{searchString}') > 0 
                 OR INSTR(Employees.Password, '{searchString}') > 0
                 OR INSTR(Employees.Name, '{searchString}') > 0
                 OR INSTR(Employees.Role, '{searchString}') > 0";

            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {

                    if (!reader.HasRows) return employees;

                    while (reader.Read())
                    {
                        employees.Add(new EmployeeViewModel
                        {
                            Id = reader.GetInt32(0),
                            Login = reader.GetString(1),
                            Password = reader.GetString(2),
                            Name = reader.GetString(3),
                            Role = reader.GetString(4),
                            DepartmentName = reader.IsDBNull(5) ? null : reader.GetString(5)
                        });
                    }

                    return employees;
                }
            }
        }

        public EmployeeCreateViewModel? Create(EmployeeCreateViewModel employee)
        {
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();

                command.Connection = connection;
                try
                {
                    var departmentIdToString = employee.DepartmentId == null ? "NULL" : employee.DepartmentId.ToString();
                    command.CommandText = $@"INSERT INTO Employees (Login, Password, Name, Role, DepartmentID) 
                                          VALUES ('{employee.Login}', '{employee.Password}', 
                                          '{employee.Name}', '{employee.Role}' , {departmentIdToString})";

                    command.ExecuteNonQuery();

                    return employee;
                }
                catch
                {
                    return null;
                }
            }
        }

        public EmployeeUpdateViewModel? Update(EmployeeUpdateViewModel employee)
        {
            var departmentIdToString = employee.DepartmentId == null ? "NULL" : employee.DepartmentId.ToString();
            var sqlExpression = $@"UPDATE Employees SET Login='{employee.Login}', 
                                Password='{employee.Password}', Name='{employee.Name}', 
                                Role='{employee.Role}', DepartmentID={departmentIdToString} WHERE _id={employee.Id}";

            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                try
                {
                    SqliteCommand command = new SqliteCommand(sqlExpression, connection);

                    command.ExecuteNonQuery();

                    return employee;
                }
                catch
                {
                    return null;
                }
            }
        }

        public int? Delete(int id)
        {
            var sqlExpression = $"DELETE FROM Employees WHERE _id={id}";

            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                try
                {
                    SqliteCommand command = new SqliteCommand(sqlExpression, connection);

                    command.ExecuteNonQuery();

                    return id;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}