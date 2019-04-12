using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webmonitor.Models
{
    public class SigfoxPayload
    {
        [Key]
        public int Id { get; set; }
        public string device { get; set; }
        public int timestamp { get; set; }
        public string type { get; set; }
        public int seqNumber { get; set; }
        public bool ack { get; set; }
        public string value { get; set; }
    }
}