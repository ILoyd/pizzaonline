using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PizzaOnline.Dal.Entities;

namespace PizzaOnline.Dal
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted)
                {
                    var entity = entry.Entity as Dal.Entities.Pizza;

                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                    }

                    entry.State = EntityState.Modified;

                }
            }

            return await base.SaveChangesAsync();
        }

        public async Task<int> CustomSaveChangesAsync(bool forceUpdate, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (forceUpdate)
                {
                    HandleConcurrency();
                    return await SaveChangesAsync();
                }
                else
                    throw;
            }
        }

        private void HandleConcurrency()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Dal.Entities.Pizza)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            proposedValues[property] = proposedValue;
                        }

                    if(databaseValues != null)
                        entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pizza>().HasData(
                new Pizza("Margherita") { Id = 1, UnitPrice = 1000, Description= "seasoned-tomato sauce, mozzarella", ImageUrl="/Images/pizza_1.jpg", Size=Size.Small, StuffedCrust = StuffedCrust.Normal },
                new Pizza("Ham & Sweetcorn") { Id = 2, UnitPrice = 1500, Description= "tomato sauce, ham, sweetcorn, mozzarella",  ImageUrl = "/Images/pizza_2.jpg", Size = Size.Medium, StuffedCrust = StuffedCrust.Cheese },
                new Pizza("SonGoKu") { Id = 3, UnitPrice = 2000, Description = "tomato sauce, ham, mushrooms, mozzarella, sweetcorn", ImageUrl = "/Images/pizza_3.jpg", Size = Size.Large, StuffedCrust = StuffedCrust.Normal },
                new Pizza("Hungarian") { Id = 4, UnitPrice = 3000, Description = "tomato sauce, salami, bacon, onions, pepperoni pepper, mozzarella", ImageUrl = "/Images/pizza_4.jpg", Size = Size.Medium, StuffedCrust = StuffedCrust.Jalapeno },
                new Pizza("Garlic-Cream") { Id = 5, UnitPrice = 3000, Description = "cream base, mozzarella, garlic, tomato slices, sausage, ham", ImageUrl = "/Images/pizza_5.jpg" , Size = Size.Small, StuffedCrust = StuffedCrust.Normal },
                new Pizza("Bolognese") { Id = 6, UnitPrice = 2500, Description = "bolognese sauce, mozzarella", ImageUrl = "/Images/pizza_6.jpg", Size = Size.Medium, StuffedCrust = StuffedCrust.Normal },
                new Pizza("Four Cheese") { Id = 7, UnitPrice = 2300, Description = "tomato sauce, marble cheese, parmesan, cheddar, mozzarella", ImageUrl = "/Images/pizza_7.jpg", Size = Size.Large, StuffedCrust = StuffedCrust.Beacon }
            );

            modelBuilder.Entity<Pizza>()
           .HasQueryFilter(p => !p.IsDeleted)
           .HasMany(p => p.Orders)
           .WithMany(o => o.Pizzas)
           .UsingEntity<OrderItem>(
               j => j
                   .HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Restrict),
               j => j
                   .HasOne(oi => oi.Pizza)
                   .WithMany(p => p.PizzaOrders)
                   .HasForeignKey(oi => oi.PizzaId),
               j =>
               {
                   j.HasKey(oi => oi.Id);
               });

            modelBuilder
            .Entity<Pizza>()
            .Property(e => e.Size)
            .HasConversion(new EnumToStringConverter<Size>());            
            
            modelBuilder
            .Entity<Pizza>()
            .Property(e => e.StuffedCrust)
            .HasConversion(new EnumToStringConverter<StuffedCrust>());

            modelBuilder.Entity<Pizza>()
            .Property(p => p.RowVersion)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsConcurrencyToken();

            modelBuilder.Entity<OrderItem>()
                .HasQueryFilter(oi => !oi.Pizza.IsDeleted);

            modelBuilder.Entity<Pizza>()
                .HasKey(p => p.Id);            
            
            modelBuilder.Entity<Pizza>()
                .HasAlternateKey(p => p.Name);            
            
            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);            
            
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.Id);
        }

        public DbSet<Pizza> Pizzas => Set<Pizza>();
        public DbSet<Order> Orders => Set<Order>();
    }
}