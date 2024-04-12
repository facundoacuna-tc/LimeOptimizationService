using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOS.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace LOS.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<VersionInfo> VersionInfo { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Sensor> Sensor { get; set; }
        public DbSet<SensorType> SensorType { get; set; }
        public DbSet<SensorValue> SensorValue { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VersionInfo>().HasNoKey();

            modelBuilder.Entity<User>().HasKey(x => x.Id);

            modelBuilder.Entity<Sensor>().HasKey(x => x.Id);

            modelBuilder.Entity<SensorType>().HasKey(x => x.Id);

            modelBuilder.Entity<SensorValue>().HasNoKey();
        }

    }
}
