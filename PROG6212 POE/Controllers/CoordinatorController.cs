using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Data;
using PROG6212_POE.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly AppDbContext _context;

        public CoordinatorController(AppDbContext context)
        {
            _context = context;
        }

        // Display all pending claims
        public async Task<IActionResult> CoordinatorDashboard()
        {
            var pendingClaims = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.Reviews)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(pendingClaims);
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
                Comment = "Approved by coordinator",
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CoordinatorDashboard));
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
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
                Comment = "Rejected by coordinator",
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CoordinatorDashboard));
        }
    }
}
