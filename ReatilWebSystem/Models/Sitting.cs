using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using BitByByte.Validation;

namespace BitByByte.Models
{
    public class Sitting
    {
        [Key]
        public int SittingId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [EndTimeValidation]
        public DateTime EndTime { get; set; }

        public int CurrentCapacity { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public sittingStatus Status { get; set; } = sittingStatus.Open;
        [NotMapped]
        public List<SelectListItem>? SittingStatusList { get; set; }
    }

    public enum sittingStatus
    {
        Open,
        Closed,
    }
}