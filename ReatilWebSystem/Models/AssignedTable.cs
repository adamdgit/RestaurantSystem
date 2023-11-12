using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitByByte.Models
{
    public class AssignedTable
    {
        [Key]
        public int AssignedTableId { get; set; }

        [Required]
        public int TableId { get; set; }

        [ForeignKey("TableId")]
        public virtual TableInfo TableInfo { get; set; }

        [Required]
        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; }

        [Required]
        public int SittingId { get; set; }

        [ForeignKey("SittingId")]
        public virtual Sitting Sitting { get; set; }
    }
}