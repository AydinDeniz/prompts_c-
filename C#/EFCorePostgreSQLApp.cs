
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class EFCorePostgreSQLApp
{
    public static void Main(string[] args)
    {
        using (var context = new AppDbContext())
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Perform CRUD operations
            CreateSampleData(context);
            ReadData(context);
            UpdateData(context, 1, "Updated Product Name");
            DeleteData(context, 2);
            ComplexQuery(context);
        }
    }

    private static void CreateSampleData(AppDbContext context)
    {
        var category = new Category { Name = "Electronics" };
        var products = new List<Product>
        {
            new Product { Name = "Laptop", Price = 1200, Category = category },
            new Product { Name = "Smartphone", Price = 800, Category = category }
        };
        context.Categories.Add(category);
        context.Products.AddRange(products);
        context.SaveChanges();
    }

    private static void ReadData(AppDbContext context)
    {
        var products = context.Products.Include(p => p.Category).ToList();
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Name} ({product.Category.Name}) - ${product.Price}");
        }
    }

    private static void UpdateData(AppDbContext context, int productId, string newName)
    {
        var product = context.Products.Find(productId);
        if (product != null)
        {
            product.Name = newName;
            context.SaveChanges();
        }
    }

    private static void DeleteData(AppDbContext context, int productId)
    {
        var product = context.Products.Find(productId);
        if (product != null)
        {
            context.Products.Remove(product);
            context.SaveChanges();
        }
    }

    private static void ComplexQuery(AppDbContext context)
    {
        var result = context.Products
            .Where(p => p.Price > 500)
            .GroupBy(p => p.Category.Name)
            .Select(g => new
            {
                Category = g.Key,
                AveragePrice = g.Average(p => p.Price),
                ProductCount = g.Count()
            })
            .ToList();

        foreach (var item in result)
        {
            Console.WriteLine($"Category: {item.Category}, Average Price: {item.AveragePrice}, Product Count: {item.ProductCount}");
        }
    }
}

// Entity Framework Core context and model classes

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=EFCoreSample;Username=yourusername;Password=yourpassword");
    }
}

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; }
}

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public List<Product> Products { get; set; }
}
