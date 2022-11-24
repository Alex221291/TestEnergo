namespace TestEnergo.ViewModels.EmployeeViewModels
{
    public class EmployeeUpdateViewModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public int? DepartmentId { get; set; }
    }
}
