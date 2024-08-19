using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using BitByByte.Data;
using Microsoft.AspNetCore.Identity;

namespace BitByByte.Models
{
    public class Area
    {
        [Key]
        public int AreaId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}