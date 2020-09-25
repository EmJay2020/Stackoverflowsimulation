using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using EFManytoMany.data;
using Microsoft.Extensions.Configuration;

namespace EFManytoMany.web.Controllers
{
    public class AuthController : Controller
    {
        private string _connectionString;
        public AuthController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(string email, string password)
        {
            var repos = new UserRepository(_connectionString);
            repos.SignUp(email, password);
            return Redirect("/auth/signup");
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            var repo = new UserRepository(_connectionString);
            var user = repo.Login(email, password);
            if(user== null)
            {
                TempData["error"] = "Invalid Login";
                return Redirect("/auth/login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/home/home");
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/home");
        }
    }
}