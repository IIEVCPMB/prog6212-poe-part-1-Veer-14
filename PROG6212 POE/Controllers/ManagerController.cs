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

        
        public async Task<IActionResult> ManagerDashboard()
        {
            var claims = await _context.Claims
                .Include(c => c.User)
                .Include(c => c.Attachments)
                .Include(c => c.Reviews)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimID == id);

            if (claim == null)
                return NotFound();

            claim.Status = ClaimStatus.Approved;

            var review = new Review
            {
                ClaimID = claim.ClaimID,
                Decision = ReviewDecision.Approved,
                Comment = "Approved by manager",
                ReviewDate = System.DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagerDashboard));
        }

        
        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimID == id);

            if (claim == null)
                return NotFound();

            claim.Status = ClaimStatus.Rejected;

            var review = new Review
            {
                ClaimID = claim.ClaimID,
                Decision = ReviewDecision.Rejected,
                Comment = "Rejected by manager",
                ReviewDate = System.DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagerDashboard));
        }
    }
}
