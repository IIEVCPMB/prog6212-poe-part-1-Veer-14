using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Models;
using PROG6212_POE.Data;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PROG6212_POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LecturerController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Lecturer Dashboard View
        public async Task<IActionResult> LecturerDashboard()
        {
            // Include related attachments and user
            var claims = await _context.Claims
                .Include(c => c.Attachments)
                .Include(c => c.User)
                .ToListAsync();

            return View(claims);
        }

        // Display Create Claim Form
        public IActionResult NewClaim()
        {
            return View();
        }

        // Handle Claim Submission (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewClaim(Claim claim, IFormFile? FileUpload)
        {
            if (!ModelState.IsValid)
                return View(claim);

            try
            {
                // Simulate logged-in user (replace with real authentication later)
                claim.UserID = 1; // TEMPORARY for testing
                claim.Status = ClaimStatus.Pending;
                claim.DateSubmitted = DateTime.Now;

                // Initialize attachments collection if null
                if (claim.Attachments == null)
                    claim.Attachments = new List<ClaimAttachment>();

                // Handle File Upload
                if (FileUpload != null && FileUpload.Length > 0)
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var ext = Path.GetExtension(FileUpload.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(ext))
                    {
                        TempData["Error"] = "Only PDF, DOCX, and XLSX files are allowed.";
                        return View(claim);
                    }

                    var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsDir);

                    var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsDir, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await FileUpload.CopyToAsync(stream);
                    }

                    // Add attachment to claim's navigation property
                    claim.Attachments.Add(new ClaimAttachment
                    {
                        FileName = FileUpload.FileName,
                        FilePath = $"/uploads/{uniqueFileName}"
                    });
                }

                // Save claim (with attachments)
                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Claim submitted successfully!";
                return RedirectToAction("LecturerDashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(claim);
            }
        }
    }
}
