using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using E_Commerce.Domain;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.OrdersModule;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Persistence.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Data.DataSeed
{
    public class DataInitializer : IDataInitializer
    {
        private readonly EcomerceDbContext dbcontext;
        public DataInitializer(EcomerceDbContext _dbcontext)
        {
            dbcontext= _dbcontext;
        }
        public async Task InitializeAsync()
        {
            try
            {
                var hasProducts = await dbcontext.Products.AnyAsync();
                var hasBrands = await dbcontext.Brands.AnyAsync();
                var hasCategories = await dbcontext.Categories.AnyAsync();
                var hasDeliveryMethods = await dbcontext.DeliveryMethods.AnyAsync();
                if (hasBrands && hasProducts && hasCategories && hasDeliveryMethods) return;

                if (!hasBrands)
                    await SeedDataFromJson<Brand, int>("brands.json", dbcontext.Brands);

                if (!hasCategories)
                    await SeedDataFromJson<Category, int>("categories.json", dbcontext.Categories);

                await dbcontext.SaveChangesAsync();
                if (!hasProducts)
                    await SeedDataFromJson<Product, int>("products.json", dbcontext.Products);

                if (!hasDeliveryMethods)
                    await SeedDataFromJson<DeliveryMethod, int>("delivery.json", dbcontext.Set<DeliveryMethod>());

                await dbcontext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during data seeding: {ex.Message}");
            }
        }
        private async Task SeedDataFromJson<T, TKey>(string filename, DbSet<T> dbSet) where T : BaseEntity<TKey>
        {

            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "DataSeed", "JSONFiles", filename);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file {filename} was not found at path {filePath}.");

            try
            {
                using var dataStream = File.OpenRead(filePath);
                var data = await JsonSerializer.DeserializeAsync<List<T>>(dataStream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                if (data != null)
                {
                    await dbSet.AddRangeAsync(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while seeding data from {filename}: {ex.Message}");
                return;
            }
        }

    }
}
