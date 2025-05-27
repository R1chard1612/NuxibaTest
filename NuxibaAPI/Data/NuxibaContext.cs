using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaAPI.Data
{
    public class NuxibaContext : DbContext
    {
        public NuxibaContext(DbContextOptions<NuxibaContext> options) : base(options)
        { }

        public DbSet<Login> Logins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Area> Areas { get; set; }
    }
}
