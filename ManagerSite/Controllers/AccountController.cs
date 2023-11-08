using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Site2.Models;
using System.Diagnostics;

namespace Site2.Controllers
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
            return Content("<h1>Access denied: This page require manager role</h1>", "text/html");
        }
    }
}