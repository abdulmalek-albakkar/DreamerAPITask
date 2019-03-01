using Dreamer.IDomain.Repositories;
using Dreamer.Models.Main;
using Dreamer.SharedContext.Repository;
using Dreamer.SqlServer.Database;
using Dreamer.SqlServer.Tools;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreamer.Domain.Repositories
{
    public class SeedingRepository : DreamerRepository, ISeedingRepository
    {
        public SeedingRepository(DreamerDbContext dreamerDbContext) : base(dreamerDbContext)
        {

        }
        public bool Clear()
        {
            try
            {
                List<string> mainNames = new List<string> { "OrderProducts", "ProductCategories", "CartItems", "Categories", "Orders", "OrderShipments", "Products" };
                List<string> securityNames = new List<string> { "DreamerUsers" };
                foreach (var table in mainNames)
                {
                    string query = "Delete From Main." + table;
                    DreamerDbContext.Database.ExecuteSqlCommand(new RawSqlString(query));
                }
                foreach (var table in securityNames)
                {
                    string query = "Delete From Security." + table;
                    DreamerDbContext.Database.ExecuteSqlCommand(new RawSqlString(query));
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
        public bool Seed()
        {
            if (!Clear()) return false;
            try
            {
                List<CategorySet> categories = new List<CategorySet>();
                for (int i = 0; i <= 10; i++)
                {
                    categories.Add(new CategorySet { Name = RandomString() });
                }
                DreamerDbContext.Categories.AddRange(categories);
                DreamerDbContext.SaveChanges();

                List<ProductSet> products = new List<ProductSet>();
                for (int i = 0; i <= 100; i++)
                {
                    products.Add(new ProductSet
                    {
                        Name = RandomString(),
                        Price = GetRandomNumber(),
                        ProductCategories = categories.GetRange(GetRandomNumber(0, 9), 2).Select(c => new ProductCategorySet { CategoryId = c.Id }).ToList()
                    });
                }
                DreamerDbContext.Products.AddRange(products);
                DreamerDbContext.SaveChanges();

                DreamerDbContext.DreamerUsers.Add(new Models.Security.DreamerUserSet { Email = "admin@dreamer.com", Password = "admin".ToSHA256(), Name = "Admin", Role = Models.Enums.DreamerUserRoles.Administrator });
                DreamerDbContext.DreamerUsers.Add(new Models.Security.DreamerUserSet { Email = "customer@dreamer.com", Password = "customer".ToSHA256(), Name = "Customer", Role = Models.Enums.DreamerUserRoles.Customer });
                DreamerDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }






        private static Random random = new Random();
        public static string RandomString(int length = 20)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static int GetRandomNumber(int min = 1000, int max = 20000)
        {
            lock (random) // synchronize
            {
                return random.Next(min, max);
            }
        }
    }
}
