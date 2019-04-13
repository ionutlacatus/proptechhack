using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webmonitor.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public DateTime ReservationStartUtc { get; set; }
        public DateTime ReservationEndUtc { get; set; }
        public string QRCode { get; set; }
        public string UserName { get; set; }
        public int SensorId { get; set; }
        [ForeignKey("SensorId")]
        public virtual Sensor Sensor { get; set; }
    }
}