using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webmonitor.Models
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public string DeviceId { get; set; }
        public string OwnerId { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }

        [NotMapped]
        public string IsReserved { get; set; }
    }
}
