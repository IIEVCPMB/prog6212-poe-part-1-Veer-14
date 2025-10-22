using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System.Linq;

namespace PROG6212_POE.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // Display login page
        public IActionResult Login()
        {
            return View();
        }

        // Handle login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginUser(string Email, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email or password.";
                return RedirectToAction("Login");
            }

            // Store user info in session
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetInt32("UserID", user.UserID);

            // Redirect based on role
            if (user.Role == UserRole.Lecturer)
                return RedirectToAction("LecturerDashboard", "Lecturer");
            else
                return RedirectToAction("CoordinatorDashboard", "Coordinator");
        }
    }
}
