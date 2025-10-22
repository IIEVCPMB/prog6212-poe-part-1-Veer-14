using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System.Threading.Tasks;
using System.Linq;

namespace PROG6212_POE.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(string FirstName, string LastName, string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "Email and Password are required.";
                return RedirectToAction("Register");
            }

            // Check if email already exists
            if (_context.Users.Any(u => u.Email == Email))
            {
                TempData["ErrorMessage"] = "Email is already registered.";
                return RedirectToAction("Register");
            }

            var newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password, // For real apps, hash this!
                Role = UserRole.Lecturer
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Store logged-in user info in session
            HttpContext.Session.SetInt32("UserID", newUser.UserID);
            HttpContext.Session.SetString("UserFullName", $"{newUser.FirstName} {newUser.LastName}");

            TempData["SuccessMessage"] = "Successfully registered!";

            // Redirect to homepage after registration
            return RedirectToAction("Index", "HomePage");
        }
    }
}
