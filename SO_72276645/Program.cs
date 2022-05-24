using Microsoft.EntityFrameworkCore;
using SO_72276645;

var options = new DbContextOptionsBuilder<MyDbContext>().UseInMemoryDatabase(databaseName: "database")
                                                        .Options;

using var context = new MyDbContext(options);

var paris = new City
{
    Name = "Paris",
    ZipCode = "75000"
};

var nyork = new City
{
    Name = "Nyork",
    ZipCode = "XXXXX"
};

var bill = new Person
{
    Name = "Bill",
    CityOfBirth = paris,
    Age = 38
};

var missHilton = new Person
{
    Name = "Miss Paris",
    CityOfBirth = nyork,
    Age = 22
};

var tom = new Pet { Name = "Tom", Type = "Mammal", Owner = missHilton };
var tinkerbell = new Pet { Name = "Feu Tinkerbell", Type = "Mammal", Owner = missHilton };

context.Pets.Add(tom);
context.Pets.Add(tinkerbell);

missHilton.Pets = new List<Pet> { tom, tinkerbell };

context.Cities.Add(nyork);
context.Cities.Add(paris);

context.Persons.Add(bill);
context.Persons.Add(missHilton);

context.SaveChanges();

Console.Write("#1 ");
Console.WriteLine(string.Join(", ", context.Persons
                                           .DeepSearch("Paris")));

Console.Write("#2 ");
Console.WriteLine(string.Join(", ", context.Persons
                                           .Where(p => p.Age > 30)
                                           .DeepSearch(context, "Paris")));

Console.Write("#3 ");
Console.WriteLine(string.Join(", ", context.Pets
                                           .DeepSearch("Paris")));

Console.Write("#4 ");
Console.WriteLine(string.Join(", ", context.Persons
                                           .DeepSearch("Mammal")));

Console.Write("#5 ");
Console.WriteLine(string.Join(", ", context.Pets
                                           .DeepSearch<Pet, int>(i => i == 22)));

Console.ReadLine();