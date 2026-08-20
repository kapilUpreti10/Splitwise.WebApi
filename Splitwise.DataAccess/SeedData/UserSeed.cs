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
                    CreatedAt = new DateTime(2026, 1, 1,0,0,0,DateTimeKind.Utc),
                    PasswordHash= "AQAAAAIAAYagAAAAEAZx6oWb1/m9EvmbWMcQZvQYteRtB4w2AqiaNoqO+Eqfc2x/mpr1Mk1S0A3OOxza0g==",
                    SecurityStamp = "security-stamp-user-001",
                    ConcurrencyStamp = "concurrency-stamp-user-001"

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
                    CreatedAt = new DateTime(2026, 1, 2,0,0,0,DateTimeKind.Utc),
                    PasswordHash= "AQAAAAIAAYagAAAAEKbVcYcgbeTr5H12+wl7GsK3jhMTYnvxt7NYbGgSJRvbVbvvHJsgm29TLgKCkWarXw==",
                    SecurityStamp = "security-stamp-user-002",
                    ConcurrencyStamp = "concurrency-stamp-user-002"

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
                    CreatedAt = new DateTime(2026, 1, 3,0,0,0,DateTimeKind.Utc),
                    PasswordHash= "AQAAAAIAAYagAAAAEMVeU+XMblpEyNLe8wlsQwvZiR1twdlvhi//vM1XVb2eaRfX0uF8H3ti0NYsvHwRTA==",
                    SecurityStamp = "security-stamp-user-003",
                    ConcurrencyStamp = "concurrency-stamp-user-003"

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
                    CreatedAt = new DateTime(2026, 1, 3,0,0,0,DateTimeKind.Utc),
                    PasswordHash= "AQAAAAIAAYagAAAAEPKhSgXj7sWfG0Iqhy++5f6LUicpWcMJuFGbRVWtBGajmFsUWZyJRDna4fuwEaDn5A==",

                    // we should add this to get rid of error as since it is guid type and each time migration is created different type of guid 
                    // will be generated so it detects data has changed eventhough we havent so we have to give fix value 

                    SecurityStamp = "security-stamp-user-004",
                    ConcurrencyStamp = "concurrency-stamp-user-004"

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
                    CreatedAt = new DateTime(2026, 1, 3,0,0,0,DateTimeKind.Utc),
                    PasswordHash= "AQAAAAIAAYagAAAAEDc2EU4wGZbdZLQxbpdxc8HwLQV7u0foLPO71WpCv190whBUZnLAC01feheDq3RcRA==",
                    SecurityStamp = "security-stamp-user-005",
                    ConcurrencyStamp = "concurrency-stamp-user-005"
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