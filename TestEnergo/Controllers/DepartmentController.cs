using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestEnergo.Constants;
using TestEnergo.Services;
using TestEnergo.ViewModels.DepartmentViewModels;

namespace TestEnergo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [Authorize(Roles = nameof(RolesConst.Admin) + ", " + nameof(RolesConst.Employee))]
        [Route("GetAll")]
        [HttpGet]
        public List<DepartmentViewModel> GetAll() => _departmentService.GetAll();

        [Authorize(Roles = nameof(RolesConst.Admin) + ", " + nameof(RolesConst.Employee))]
        [Route("GetById")]
        [HttpGet]
        public ObjectResult GetById(int id)
        {
            var department = _departmentService.GetById(id);
            if (department == null) return BadRequest(department);
            return Ok(department);
        }

        [Authorize(Roles = nameof(RolesConst.Admin) + ", " + nameof(RolesConst.Employee))]
        [Route("GetAllPaths")]
        [HttpGet]
        public List<string> GetGetAllPathsAll() => _departmentService.GetAllPaths();

        [Authorize(Roles = nameof(RolesConst.Admin))]
        [Route("Create")]
        [HttpPost]
        public ObjectResult Create(DepartmentCreateViewModel createDepartment)
        {
            var department = _departmentService.Create(createDepartment);
            if (department == null) return BadRequest(department);
            return Ok(department);
        }

        [Authorize(Roles = nameof(RolesConst.Admin))]
        [Route("Update")]
        [HttpPut]
        public ObjectResult Update(DepartmentUpdateViewModel updateDepartment)
        {
            var department = _departmentService.Update(updateDepartment);
            if (department == null) return BadRequest(department);
            return Ok(department);
        }

        [Authorize(Roles = nameof(RolesConst.Admin))]
        [Route("EditParentDepartment")]
        [HttpPut]
        public ObjectResult EditParentDepartment(EditParentDepartmentViewModel model)
        {
            var newParent = _departmentService.EditParentDepartment(model);
            if (newParent == null) return BadRequest(newParent);
            return Ok(newParent);
        }

        [Authorize(Roles = nameof(RolesConst.Admin))]
        [Route("Delete")]
        [HttpDelete]
        public ObjectResult Delete(int deleteId)
        {
            var id = _departmentService.Delete(deleteId);
            if (id == null) return BadRequest(id);
            return Ok(id);
        }
    }
}
