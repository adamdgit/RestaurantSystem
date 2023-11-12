using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitByByte.Models
{
    public class TableInfo
    {
        [Key]
        public int TableId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Seats { get; set; }

        [Required]
        public string Availability { get; set; }

        [Required]
        public int AreaId { get; set; }

        [ForeignKey("AreaId")]
        public virtual Area Area { get; set; }
    }
}