using Microsoft.EntityFrameworkCore;

namespace SO_72276645;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }

    public DbSet<Person> Persons { get; set; }
    
    public DbSet<Pet> Pets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>()
                    .HasOne(p => p.CityOfBirth);

        modelBuilder.Entity<Pet>()
                    .HasOne(p => p.Owner)
                    .WithMany(p => p.Pets);
    }
}

public class Pet
{
    public int PetId { get; set; }

    public string Name { get; set; }
    public Person Owner { get; set; }
    public string Type { get; set; }

    public override string ToString() => Name;
}

public class Person
{
    public int PersonId { get; set; }
    
    public int Age { get; set; }
    public City CityOfBirth { get; set; }
    public string Name { get; set; }
    public List<Pet> Pets { get; set; }

    public override string ToString() => Name;
}

public class City
{
    public int CityId { get; set; }
    public string Name { get; set; }
    public string ZipCode { get; set; }

    public override string ToString() => Name;
}