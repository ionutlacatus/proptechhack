using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webmonitor.Models
{
    public class SigfoxDb1 : DbContext
    {
        public SigfoxDb1(DbContextOptions<SigfoxDb1> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<SigfoxPayload> SigFoxPayloads { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
