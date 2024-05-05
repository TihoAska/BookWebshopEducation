using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BookWebshopEducation.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace BookWebshopEducation.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    // Always use plural here
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<ShoppingCart> ShoppingCart { get; set; }

    // Seed data here (there are other ways of doing this)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
