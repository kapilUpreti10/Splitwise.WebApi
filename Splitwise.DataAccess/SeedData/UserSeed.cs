using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwise.Models;

namespace Splitwise.DataAccess.SeedData
{
    public static class UserSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();

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
                CreatedAt = new DateTime(2026, 1, 1)
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
                CreatedAt = new DateTime(2026, 1, 2)
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
                CreatedAt = new DateTime(2026, 1, 3)
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
                CreatedAt = new DateTime(2026, 1, 3)
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
                CreatedAt = new DateTime(2026, 1, 3)
            };

            user1.PasswordHash = passwordHasher.HashPassword(user1, "kapil@example.com");
            user2.PasswordHash = passwordHasher.HashPassword(user2, "pratap@example.com");
            user3.PasswordHash = passwordHasher.HashPassword(user3, "niraj@example.com");
            user4.PasswordHash = passwordHasher.HashPassword(user4, "pariskar@example.com");
            user5.PasswordHash = passwordHasher.HashPassword(user5, "parbat@example.com");




            modelBuilder.Entity<ApplicationUser>().HasData(
                user1,user2,user3,user4,user5
            );
        }
    }
}