using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TestEnergo.Constants;
using TestEnergo.ViewModels.DepartmentViewModels;

namespace TestEnergo.Services
{
    public interface IDepartmentService
    {
        public List<DepartmentViewModel> GetAll();
        public DepartmentViewModel? GetById(int id);
        public List<string> GetAllPaths();
        public DepartmentCreateViewModel? Create(DepartmentCreateViewModel department);
        public DepartmentUpdateViewModel? Update(DepartmentUpdateViewModel department);
        public EditParentDepartmentViewModel? EditParentDepartment(EditParentDepartmentViewModel model);
        public int? Delete(int id);
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly MyConnection _myConnection;

        public DepartmentService(IOptions<MyConnection> myConnection)
        {
            _myConnection = myConnection.Value;
        }

        public List<DepartmentViewModel> GetAll()
        {
            var sqlExpression =
                @"SELECT d._id, d.Name, Departments.Name
                FROM Departments d
                LEFT JOIN Departments ON Departments._id = d.DepartmentID";
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var departments = new List<DepartmentViewModel>();

                    if (!reader.HasRows) return departments;

                    while (reader.Read())
                    {
                        departments.Add(new DepartmentViewModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            DepartmentName = reader.IsDBNull(2) ? null : reader.GetString(2)
                        });
                    }

                    return departments;
                }
            }
        }

        public DepartmentViewModel? GetById(int id)
        {
            var sqlExpression =
                $@"SELECT d._id, d.Name, Departments.Name
                 FROM Departments d
                 LEFT JOIN Departments ON Departments._id = d.DepartmentID
                 WHERE d._id = {id}";
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return null;

                    reader.Read();

                    return new DepartmentViewModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        DepartmentName = reader.IsDBNull(2) ? null : reader.GetString(2)
                    };
                }
            }
        }

        public List<string> GetAllPaths()
        {
            var sqlExpression =
                @"WITH cte AS (
                    SELECT
                        _id,
                        DepartmentId,
                        '/' || Name AS Name
                    FROM Departments WHERE DepartmentId IS NULL
                    UNION ALL
                     SELECT
                        Departments._id,
                        Departments.DepartmentId,
                        cte.Name || '/' || Departments.Name
                    FROM cte, Departments ON Departments.DepartmentId = cte._id
                )
                SELECT Name FROM cte";
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var paths = new List<string>();

                    if (!reader.HasRows) return paths;

                    while (reader.Read())
                        paths.Add(reader.GetString(0));

                    return paths;
                }
            }
        }

        public DepartmentCreateViewModel? Create(DepartmentCreateViewModel department)
        {
            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();

                command.Connection = connection;
                try
                {
                    var departmentIdToString =
                        department.DepartmentId == null ? "NULL" : department.DepartmentId.ToString();
                    command.CommandText = $@"INSERT INTO Departments (Name, DepartmentID) 
                                          VALUES ('{department.Name}',{departmentIdToString})";

                    command.ExecuteNonQuery();

                    return department;
                }
                catch
                {
                    return null;
                }
            }
        }

        public DepartmentUpdateViewModel? Update(DepartmentUpdateViewModel department)
        {
            var departmentIdToString = department.DepartmentId == null ? "NULL" : department.DepartmentId.ToString();
            var sqlExpression =
                $@"UPDATE Departments SET Name='{department.Name}', DepartmentID={departmentIdToString}
                 WHERE _id={department.Id}";

            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                connection.Open();

                try
                {
                    SqliteCommand command = new SqliteCommand(sqlExpression, connection);

                    command.ExecuteNonQuery();

                    return department;
                }
                catch
                {
                    return null;
                }
            }
        }

        public EditParentDepartmentViewModel? EditParentDepartment(EditParentDepartmentViewModel model)
        {
            var departmentIdToString = model.ParentId == null ? "NULL" : model.ParentId.ToString();
            var sqlExpression =
                $@"UPDATE Departments SET DepartmentID={departmentIdToString}
                 WHERE _id={model.Id}";

            using (var connection = new SqliteConnection(_myConnection.DefaultConnection))
            {
                try
                {
                    connection.Open();

                    SqliteCommand command = new SqliteCommand(sqlExpression, connection);

                    command.ExecuteNonQuery();

                    return model;
                }
                catch
                {
                    return null;
                }
            }
        }

        public int? Delete(int id)
        {
            var sqlExpression = $"DELETE FROM Departments WHERE _id={id}";

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