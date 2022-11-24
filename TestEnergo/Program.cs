using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.Sqlite;
using TestEnergo.Constants;
using TestEnergo.Services;

var builder = WebApplication.CreateBuilder(args);

using (var connection = new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    connection.Open();
    SqliteCommand command = new SqliteCommand();
    command.Connection = connection;

    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Departments'";
    if (command.ExecuteScalar() == null)
    {
        command.CommandText = @"CREATE TABLE Departments(
                              _id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                               Name TEXT NOT NULL,
                               DepartmentID INTEGER CHECK (DepartmentId != _id),
                               FOREIGN KEY (DepartmentID)  REFERENCES Departments (_id) ON DELETE SET NULL)";
        command.ExecuteNonQuery();
    }

    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Employees'";
    if (command.ExecuteScalar() == null)
    {
        command.CommandText = @"CREATE TABLE Employees(
                              _id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                               Login TEXT NOT NULL UNIQUE,
                               Password TEXT NOT NULL,
                               Name TEXT NOT NULL,
                               Role TEXT NOT NULL CHECK (Role IN ('Admin', 'Employee')),
                               DepartmentID INTEGER,
                               FOREIGN KEY (DepartmentID)  REFERENCES Departments (_id) ON DELETE SET NULL)";
        command.ExecuteNonQuery();
        command.CommandText =
            "INSERT INTO Employees (Login, Password, Name, Role) VALUES ('admin', 'admin', 'Alex', 'Admin')";
        command.ExecuteNonQuery();
    }
}
// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.AccessDeniedPath = new PathString("/Account/Login");
    });
builder.Services.Configure<MyConnection>(c => c.DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
