using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Site1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Content("<h1>Access denied: This page require admin role</h1>", "text/html");
        }
    }
}