using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System.Threading.Tasks;
using System.Linq;

namespace PROG6212_POE.Controllers
{
    public class ManagerController : Controller
    {
        private readonly AppDbContext _context;

        public ManagerController(AppDbContext context)
        {
            _context = context;
        }

        // Display all claims
        public async Task<IActionResult> ManagerDashboard()
        {
            var claims = await _context.Claims
                .Include(c => c.User) // Lecturer
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.User) // Reviewer
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Approved;

            int? userId = HttpContext.Session.GetInt32("UserID");
            var review = new Review
            {
                ClaimID = claim.ClaimID,
                UserID = userId ?? 0,
                Decision = ReviewDecision.Approved,
                Comment = "Approved by manager",
                ReviewDate = System.DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagerDashboard));
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id, string? comment)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Rejected;

            int? userId = HttpContext.Session.GetInt32("UserID");
            var review = new Review
            {
                ClaimID = claim.ClaimID,
                UserID = userId ?? 0,
                Decision = ReviewDecision.Rejected,
                Comment = comment ?? "Rejected by manager",
                ReviewDate = System.DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagerDashboard));
        }
    }
}
