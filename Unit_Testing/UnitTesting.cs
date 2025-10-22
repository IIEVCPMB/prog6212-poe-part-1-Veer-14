using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Controllers;
using PROG6212_POE.Data;
using PROG6212_POE.Models;



namespace PROG6212_POE.Tests
{
    // Minimal test ISession implementation so controllers that rely on HttpContext.Session work
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();

        public IEnumerable<string> Keys => _store.Keys;

        public string Id { get; } = Guid.NewGuid().ToString();
        public bool IsAvailable { get; } = true;

        public void Clear() => _store.Clear();

        public Task CommitAsync() => Task.CompletedTask;

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync() => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key) => _store.Remove(key);

        public void Set(string key, byte[] value) => _store[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);
    }

    // Helper to create an In-Memory AppDbContext for tests
    public static class TestHelper
    {
        public static AppDbContext CreateInMemoryContext(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // Convenience to set an int into the session (GetInt32/SetInt32 uses int represented as bytes)
        public static void SetSessionInt(ISession session, string key, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            session.Set(key, bytes);
        }

        public static int? GetSessionInt(ISession session, string key)
        {
            if (session.TryGetValue(key, out var bytes))
            {
                return BitConverter.ToInt32(bytes, 0);
            }
            return null;
        }
    }

    public class ClaimsFlowTests
    {
        [Fact]
        public async Task AddClaim_Successful()
        {
            using var ctx = TestHelper.CreateInMemoryContext();

            var claim = new Claim
            {
                UserID = 1,
                ClaimMonth = "October 2025",
                HoursWorked = 10,
                HourlyRate = 100M,
                Notes = "Test note"
            };

            ctx.Claims.Add(claim);
            await ctx.SaveChangesAsync();

            Assert.True(claim.ClaimID > 0, "Claim should have an ID assigned by the DB");
            Assert.Equal(ClaimStatus.Pending, claim.Status);
            var fetched = await ctx.Claims.FindAsync(claim.ClaimID);
            Assert.NotNull(fetched);
            Assert.Equal("October 2025", fetched.ClaimMonth);
        }

        [Fact]
        public async Task AddAttachment_SavesRecord()
        {
            using var ctx = TestHelper.CreateInMemoryContext();

            var claim = new Claim
            {
                UserID = 1,
                ClaimMonth = "October 2025",
                HoursWorked = 5,
                HourlyRate = 200M
            };
            ctx.Claims.Add(claim);
            await ctx.SaveChangesAsync();

            var attachment = new ClaimAttachment
            {
                ClaimID = claim.ClaimID,
                FileName = "doc.pdf",
                FilePath = "/uploads/doc.pdf"
            };

            ctx.ClaimAttachments.Add(attachment);
            await ctx.SaveChangesAsync();

            var attachments = await ctx.ClaimAttachments.Where(a => a.ClaimID == claim.ClaimID).ToListAsync();
            Assert.Single(attachments);
            Assert.Equal("doc.pdf", attachments[0].FileName);
            Assert.Equal("/uploads/doc.pdf", attachments[0].FilePath);
        }

        [Fact]
        public async Task ApproveClaim_ShouldChangeStatusToApproved()
        {
            // Arrange
            var context = TestHelper.CreateInMemoryContext();

            var claim = new Claim
            {
                ClaimID = 1,
                Status = ClaimStatus.Pending,
                DateSubmitted = DateTime.Now,
                ClaimMonth = "October 2025" // Required property
            };

            context.Claims.Add(claim);
            await context.SaveChangesAsync();

            var controller = new CoordinatorController(context);

            // Use TestSession instead of Mock<ISession>
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            TestHelper.SetSessionInt(session, "UserID", 42); // sample user id
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.ApproveClaim(claim.ClaimID);

            // Assert
            var updatedClaim = await context.Claims.FindAsync(claim.ClaimID);
            Assert.Equal(ClaimStatus.Approved, updatedClaim.Status);

            var review = await context.Reviews.FirstOrDefaultAsync(r => r.ClaimID == claim.ClaimID);
            Assert.NotNull(review);
            Assert.Equal(ReviewDecision.Approved, review.Decision);
        }


        [Fact]
        public async Task ManagerApprove_AddsReviewAndSetsStatus()
        {
            // Arrange
            var context = TestHelper.CreateInMemoryContext();

            var claim = new Claim
            {
                UserID = 1,
                ClaimMonth = "November 2025", // Required property
                HoursWorked = 12,
                HourlyRate = 120M,
                Notes = "Test note", // optional but good to include
                DateSubmitted = System.DateTime.Now,
                Status = ClaimStatus.Pending
            };

            context.Claims.Add(claim);
            await context.SaveChangesAsync();

            var controller = new ManagerController(context);

            // Use TestSession instead of Mock<ISession>
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            TestHelper.SetSessionInt(session, "UserID", 99); // example user id
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.ApproveClaim(claim.ClaimID);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ManagerController.ManagerDashboard), redirect.ActionName);

            var updatedClaim = await context.Claims.FindAsync(claim.ClaimID);
            Assert.Equal(ClaimStatus.Approved, updatedClaim.Status);

            var review = await context.Reviews
                .Where(r => r.ClaimID == claim.ClaimID)
                .OrderByDescending(r => r.ReviewDate)
                .FirstOrDefaultAsync();

            Assert.NotNull(review);
            Assert.Equal(ReviewDecision.Approved, review.Decision);
            Assert.Equal("Approved by manager", review.Comment); // matches current controller
        }


        [Fact]
        public async Task RejectClaim_ByCoordinator_AddsReviewAndSetsStatus()
        {
            // Arrange
            var context = TestHelper.CreateInMemoryContext();

            var claim = new Claim
            {
                UserID = 1,
                ClaimMonth = "December 2025",
                HoursWorked = 6,
                HourlyRate = 100M,
                Notes = "Test note",
                DateSubmitted = DateTime.Now,
                Status = ClaimStatus.Pending
            };

            context.Claims.Add(claim);
            await context.SaveChangesAsync();

            var controller = new CoordinatorController(context);

            // Use TestSession and SetInt32
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            session.SetInt32("UserID", 55); // <-- use built-in SetInt32
            httpContext.Session = session;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await controller.RejectClaim(claim.ClaimID);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(CoordinatorController.CoordinatorDashboard), redirect.ActionName);

            var updatedClaim = await context.Claims.FindAsync(claim.ClaimID);
            Assert.Equal(ClaimStatus.Rejected, updatedClaim.Status);

            var review = await context.Reviews
                .Where(r => r.ClaimID == claim.ClaimID)
                .OrderByDescending(r => r.ReviewDate)
                .FirstOrDefaultAsync();

            Assert.NotNull(review);
            Assert.Equal(ReviewDecision.Rejected, review.Decision);
            Assert.Equal("Rejected by coordinator", review.Comment);
            Assert.Equal(55, review.UserID); // now this will pass
        }


    }
}
