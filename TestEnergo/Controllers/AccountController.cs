using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Route("Login")]
        [HttpGet]
        public async Task<ObjectResult> Login(string login, string password)
        {
            var loginModel = new EmployeeLoginViewModel
            {
                Login = login,
                Password = password
            };
            var loginEmployee = _accountService.Login(loginModel);
            if (loginEmployee == null) return BadRequest(loginEmployee);
            await Authenticate(loginEmployee);
            return Ok(loginEmployee);

        }
        private async Task Authenticate(EmployeeViewModel employee)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, employee.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, employee.Role)
            };

            var id = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(id));
        }
    }
}
