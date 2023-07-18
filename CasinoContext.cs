using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CasinoContext : DbContext
{
    public DbSet<Player> Players => Set<Player>();

    public CasinoContext()
    {
       // Database.EnsureDeleted();
        Database.EnsureCreated();

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data source=playerData.db");
    }
}
