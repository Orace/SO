using Microsoft.EntityFrameworkCore;

namespace SO_72276645;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }

    public DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>()
                    .HasOne(p => p.CityOfBirth);
    }
}

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }
    public City CityOfBirth { get; set; }

    public override string ToString() => Name;
}

public class City
{
    public int CityId { get; set; }
    public string Name { get; set; }
    public string ZipCode { get; set; }

    public override string ToString() => Name;
}