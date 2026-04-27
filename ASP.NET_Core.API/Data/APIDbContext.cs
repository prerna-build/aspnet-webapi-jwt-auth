using ASP.NET_Core.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Core.API.Data
{
    //inherit from DbContext which is part of EntityFrameworkCore
    public class APIDbContext : DbContext  
    {
        //constructor (ctor)
        public APIDbContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {
                
        }

        //Property (prop)
        // these properties will create table in database
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }
    }
}
