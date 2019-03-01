using Dreamer.Models.Base;
using Dreamer.Models.Main;
using Dreamer.Models.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dreamer.SqlServer.Database
{
    public class DreamerDbContext : DbContext
    {
        public DbSet<CartItemSet> CartItems { get; set; }
        public DbSet<CategorySet> Categories { get; set; }
        public DbSet<OrderSet> Orders { get; set; }
        public DbSet<OrderProductSet> OrderProducts { get; set; }
        public DbSet<OrderShipmentSet> OrderShipments { get; set; }
        public DbSet<ProductSet> Products { get; set; }
        public DbSet<ProductCategorySet> ProductCategories { get; set; }
        public DbSet<DreamerUserSet> DreamerUsers { get; set; }
        public DreamerDbContext(DbContextOptions<DreamerDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Prevent Cascade
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            //Default value for creation date
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreationDate))
                    .HasDefaultValueSql("GETUTCDATE()")
                    .ValueGeneratedOnAdd()
                    .ValueGeneratedOnUpdate()
                    .Metadata.BeforeSaveBehavior = Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore;
            }


            //SerialValue of the orders
            modelBuilder.HasSequence<int>("OrdersSerialNumber", schema: "shared").StartsAt(10000).IncrementsBy(1);
            modelBuilder.Entity<OrderSet>().Property(order => order.SerialNumber).HasDefaultValueSql("NEXT VALUE FOR shared.OrdersSerialNumber");

            //Unique user emil
            modelBuilder.Entity<DreamerUserSet>().HasIndex(u => u.Email).IsUnique();

        }
    }
}
