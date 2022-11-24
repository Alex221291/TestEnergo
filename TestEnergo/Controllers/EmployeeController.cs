using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestEnergo.Constants;
using TestEnergo.Services;
using TestEnergo.ViewModels.EmployeeViewModels;

namespace TestEnergo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Roles = nameof(RolesConst.Admin) + ", " + nameof(RolesConst.Employee))]
        [Route("GetAll")]
        [HttpGet]
        public List<EmployeeViewModel> GetAll() => _employeeService.GetAll();

        [Authorize(Roles = nameof(RolesConst.Admin) + ", " + nameof(RolesConst.Employee))]
        [Route("GetDepartmentEmployees")]
        [HttpGet]
        public List<EmployeeViewModel> GetDepartmentEmployees(int departmentId) => 
            _employeeService.GetDepartmentEmployees(departmentId);

        [Authorize(Roles = nameof(RolesConst.Admin) + ", " + nameof(RolesConst.Employee))]
        [Route("Search")]
        [HttpGet]
        public List<EmployeeViewModel> Search(string? searchString) =>
            _employeeService.Search(searchString);

        [Authorize(Roles = nameof(RolesConst.Admin) + ", " + nameof(RolesConst.Employee))]
        [Route("GetById")]
        [HttpGet]
        public ObjectResult GetById(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null) return BadRequest(employee);
            return Ok(employee);
        }

        [Authorize(Roles = nameof(RolesConst.Admin))]
        [Route("Create")]
        [HttpPost]
        public ObjectResult Create(EmployeeCreateViewModel createEmployee)
        {
            var employee = _employeeService.Create(createEmployee);
            if (employee == null) return BadRequest(employee);
            return Ok(employee);
        }

        [Authorize(Roles = nameof(RolesConst.Admin))]
        [Route("Update")]
        [HttpPut]
        public ObjectResult Update(EmployeeUpdateViewModel updateEmployee)
        {
            var employee = _employeeService.Update(updateEmployee);
            if (employee == null) return BadRequest(employee);
            return Ok(employee);
        }

        [Authorize(Roles = nameof(RolesConst.Admin))]
        [Route("Delete")]
        [HttpDelete]
        public ObjectResult Delete(int deleteId)
        {
            var id = _employeeService.Delete(deleteId);
            if (id == null) return BadRequest(id);
            return Ok(id);
        }
    }
}
