using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_POE.Models
{
    public enum ReviewDecision
    {
        Pending,
        Approved,
        Rejected
    }

    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        [ForeignKey("Claim")]
        public int ClaimID { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Decision is required.")]
        [EnumDataType(typeof(ReviewDecision))]
        public ReviewDecision Decision { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Review Date")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        // Navigation properties (relationships)
        public virtual Claim? Claim { get; set; }
        public virtual User? User { get; set; }
    }
}

