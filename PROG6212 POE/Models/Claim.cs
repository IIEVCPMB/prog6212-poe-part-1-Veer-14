using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_POE.Models
{
    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }

        // Relationship to User
        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        // Claim month as a string (e.g., "October 2025")
        [Required(ErrorMessage = "Claim month is required.")]
        [StringLength(50)]
        [Display(Name = "Claim Month")]
        public string ClaimMonth { get; set; }

        [Required, Range(1, 300, ErrorMessage = "Hours must be between 1 and 300.")]
        [Display(Name = "Hours Worked")]
        public int HoursWorked { get; set; }

        [Required, Range(50, 2000, ErrorMessage = "Hourly rate must be between R50 and R2000.")]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; }

        // Total amount (computed, not stored)
        [NotMapped]
        [Display(Name = "Total Amount (R)")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount => HoursWorked * HourlyRate;

        [Required(ErrorMessage = "Claim status is required.")]
        [EnumDataType(typeof(ClaimStatus))]
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        // Optional notes or description
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual User? User { get; set; } // Link to submitting lecturer
        public virtual ICollection<Review>? Reviews { get; set; } // Link to claim reviews
        public virtual ICollection<ClaimAttachment>? Attachments { get; set; } // Link to file uploads
    }
}
