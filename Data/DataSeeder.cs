using Microsoft.EntityFrameworkCore;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ULIPDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@ulip.com",
                        PhoneNumber = "9999999999",
                        DateOfBirth = new DateTime(1980, 1, 1),
                        Address = "Admin Address",
                        PanNumber = "ADMIN12345",
                        Role = UserRole.Admin,
                        Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        IsActive = true
                    },
                    new User
                    {
                        FirstName = "Manager",
                        LastName = "User",
                        Email = "manager@ulip.com",
                        PhoneNumber = "8888888888",
                        DateOfBirth = new DateTime(1985, 1, 1),
                        Address = "Manager Address",
                        PanNumber = "MNGR123456",
                        Role = UserRole.Manager,
                        Password = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                        IsActive = true
                    },
                    new User
                    {
                        FirstName = "Agent",
                        LastName = "User",
                        Email = "agent@ulip.com",
                        PhoneNumber = "7777777777",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        Address = "Agent Address",
                        PanNumber = "AGNT123456",
                        Role = UserRole.Agent,
                        Password = BCrypt.Net.BCrypt.HashPassword("Agent@123"),
                        IsActive = true
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            if (!await context.Funds.AnyAsync())
            {
                var funds = new List<Fund>
                {
                    new Fund
                    {
                        FundName = "Equity Growth Fund",
                        FundType = FundType.Equity,
                        CurrentNAV = 10.00m,
                        ExpenseRatio = 1.25m,
                        RiskLevel = RiskLevel.High,
                        Description = "High growth equity fund for long-term wealth creation"
                    },
                    new Fund
                    {
                        FundName = "Debt Secure Fund",
                        FundType = FundType.Debt,
                        CurrentNAV = 10.00m,
                        ExpenseRatio = 0.75m,
                        RiskLevel = RiskLevel.Low,
                        Description = "Conservative debt fund for stable returns"
                    },
                    new Fund
                    {
                        FundName = "Balanced Fund",
                        FundType = FundType.Balanced,
                        CurrentNAV = 10.00m,
                        ExpenseRatio = 1.00m,
                        RiskLevel = RiskLevel.Medium,
                        Description = "Balanced fund with equity and debt allocation"
                    }
                };

                await context.Funds.AddRangeAsync(funds);
                await context.SaveChangesAsync();
            }
            else
            {
                // Update NAV values to simulate market growth (only if funds exist and haven't been updated recently)
                var funds = await context.Funds.ToListAsync();
                var random = new Random();
                
                foreach (var fund in funds)
                {
                    // Only update if NAV is exactly 10 (initial value) or if it's been more than a day since last update
                    if (fund.CurrentNAV == 10.00m || (DateTime.UtcNow - fund.UpdatedAt).TotalDays > 1)
                    {
                        // Simulate market movement: +/- 0.5% to 2%
                        var changePercent = (decimal)(random.NextDouble() * 0.015 + 0.005); // 0.5% to 2%
                        var isPositive = random.Next(0, 10) > 3; // 70% chance of positive growth
                        
                        if (fund.FundType == FundType.Equity)
                        {
                            changePercent *= 1.5m; // Equity funds have higher volatility
                        }
                        else if (fund.FundType == FundType.Debt)
                        {
                            changePercent *= 0.5m; // Debt funds are more stable
                        }
                        
                        var change = fund.CurrentNAV * changePercent * (isPositive ? 1 : -1);
                        fund.CurrentNAV = Math.Max(5.00m, fund.CurrentNAV + change); // Minimum NAV of 5
                        fund.UpdatedAt = DateTime.UtcNow;
                    }
                }
                
                await context.SaveChangesAsync();
            }
        }
    }
}
