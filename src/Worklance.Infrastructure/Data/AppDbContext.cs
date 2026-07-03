using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Entities.AuthEntities;


namespace Worklance.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


    }
}
