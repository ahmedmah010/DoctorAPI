using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<NormalUser> NormalUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<DoctorPosition> DoctorPositions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Seeds
            
            builder.Entity<IdentityRole>().HasData(
               new IdentityRole() { Id = "117d1b41-6753-4622-89e6-8126a3b7d3f0", Name = "Doctor", NormalizedName = "Doctor".ToUpper(), ConcurrencyStamp = "bc020d73-e415-4d4e-8dfe-577d81755f80" },
               new IdentityRole() { Id = "34e62595-2afc-43f9-bbcd-267773129d69", Name = "User", NormalizedName = "User".ToUpper(), ConcurrencyStamp = "23ee6cfe-4bc2-4c6c-847a-d03aa0087e1f" },
               new IdentityRole() { Id = "34e62595-8afc-43f9-bbcd-267773129d69", Name = "Admin", NormalizedName = "Admin".ToUpper(), ConcurrencyStamp = "23ee6cfe-4bc2-4c6c-847a-d03aa0087n1f" }
            );
            builder.Entity<AppUser>().HasData(
                new AppUser()
                {
                    UserName = "admin",
                    NormalizedUserName = "admin",
                    Id = "34e62595-8afc-43f9-bbcd-267773129h79",
                    Fname = "Administrator",
                    Lname = "",
                    img = "",
                    city = "",
                    PasswordHash = "AQAAAAIAAYagAAAAEAmZmuov2IZeVvYF2WGjoRZvDy9jTfPQO/jxgzikW7VsAY7TV2hqplP4iCaE+CtMew==",
                    ConcurrencyStamp = "d7f2c98f-09be-46d5-9a8b-c443ac9ba785",
                    SecurityStamp = "a8d57cd3-0bad-4286-bc89-f73b1bdde503"
                }
                );
            
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "34e62595-8afc-43f9-bbcd-267773129d69",
                UserId = "34e62595-8afc-43f9-bbcd-267773129h79"
            });

      

            base.OnModelCreating(builder);
        }
       
      


    }

    

}
