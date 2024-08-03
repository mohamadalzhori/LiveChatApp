using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server.Data;

public class AppDbContext : DbContext
{
    public DbSet<Message> Messages { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts)
    {
    }

}