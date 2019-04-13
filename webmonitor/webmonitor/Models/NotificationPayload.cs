using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webmonitor.Models
{
    public class NotificationPayload
    {
        public string userId { get; set; }
        public string deviceId { get; set; }
        public string text { get; set; }
        public int hours { get; set; }
    }
}
