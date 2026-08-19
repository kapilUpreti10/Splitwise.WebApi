    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
    using Splitwise.Models;
    using Splitwise.Utils;

    namespace Splitwise.DataAccess.SeedData
    {
        public static class UserSeed
        {
            public static void Seed(ModelBuilder modelBuilder)
            {

                //var passwordHasher = new PasswordHasher<ApplicationUser>();


                var user1 = new ApplicationUser
                {
                    Id = "user-001",
                    UserName = "kapil",
                    NormalizedUserName = "KAPIL",
                    Email = "kapil@example.com",
                    NormalizedEmail = "KAPIL@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Name = "Kapil Upreti",
                    Address = "Kathmandu",
                    CreatedAt = new DateTime(2026, 1, 1),
                    PasswordHash= "AQAAAAIAAYagAAAAEAZx6oWb1/m9EvmbWMcQZvQYteRtB4w2AqiaNoqO+Eqfc2x/mpr1Mk1S0A3OOxza0g=="

                };

                var user2 = new ApplicationUser
                {
                    Id = "user-002",
                    UserName = "Niraj",
                    NormalizedUserName = "NIRAJ",
                    Email = "niraj@example.com",
                    NormalizedEmail = "NIRAJ@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Name = "Niraj Karki",
                    Address = "Mahendranagar",
                    CreatedAt = new DateTime(2026, 1, 2),
                    PasswordHash= "AQAAAAIAAYagAAAAEKbVcYcgbeTr5H12+wl7GsK3jhMTYnvxt7NYbGgSJRvbVbvvHJsgm29TLgKCkWarXw=="

                };
                var user3 = new ApplicationUser
                {
                    Id = "user-003",
                    UserName = "Pratap",
                    NormalizedUserName = "PRATAP",
                    Email = "pratap@example.com",
                    NormalizedEmail = "PRATAP@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Name = "Pratap Kunwar",
                    Address = "Argakhanchi",
                    CreatedAt = new DateTime(2026, 1, 3),
                    PasswordHash= "AQAAAAIAAYagAAAAEMVeU+XMblpEyNLe8wlsQwvZiR1twdlvhi//vM1XVb2eaRfX0uF8H3ti0NYsvHwRTA=="

                };
                var user4 = new ApplicationUser
                {
                    Id = "user-004",
                    UserName = "Pariskar",
                    NormalizedUserName = "PARISKAR",
                    Email = "pariskar@example.com",
                    NormalizedEmail = "PARISKAR@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Name = "Pariskar Poudel",
                    Address = "Butwal",
                    CreatedAt = new DateTime(2026, 1, 3),
                    PasswordHash= "AQAAAAIAAYagAAAAEPKhSgXj7sWfG0Iqhy++5f6LUicpWcMJuFGbRVWtBGajmFsUWZyJRDna4fuwEaDn5A=="

                };
                var user5 = new ApplicationUser
                {
                    Id = "user-005",
                    UserName = "Parbat",
                    NormalizedUserName = "PARBAT",
                    Email = "parbat@example.com",
                    NormalizedEmail = "PARBAT@EXAMPLE.COM",
                    EmailConfirmed = true,
                    Name = "Parbat Pandey",
                    Address = "Dang",
                    CreatedAt = new DateTime(2026, 1, 3),
                    PasswordHash= "AQAAAAIAAYagAAAAEDc2EU4wGZbdZLQxbpdxc8HwLQV7u0foLPO71WpCv190whBUZnLAC01feheDq3RcRA=="
                };

                //user1.PasswordHash = passwordHasher.HashPassword(user1, "kapil@example.com");
                // since passwordHasher generates different hash each time, we will use a fixed password hash for seeding as we cannot add dynamic
                // changing data in seeding 




                modelBuilder.Entity<ApplicationUser>().HasData(
                    user1,user2,user3,user4,user5
                );
            }
        }
    }