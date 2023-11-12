using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BitByByte.Validation;

namespace BitByByte.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public int SittingId { get; set; }

        [ForeignKey("SittingId")]
        public virtual Sitting Sitting { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Notes { get; set; }

        public Status Status { get; set; } = Status.Pending;
        [NotMapped]
        public List<SelectListItem>? StatusList { get; set; }

        [Required]
        public int GuestCount { get; set; }

        public Source ReservationSource { get; set; } = Source.Website;

        [NotMapped]
        public List<SelectListItem>? ReservationSourceList { get; set; }
    }

    public enum Source
    {
        Phone,
        Website,
        InPerson,
        MobileApp,
        Email
    }

    public enum Status
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
}
