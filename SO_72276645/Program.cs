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
    CityOfBirth = paris
};

var missHilton = new Person
{
    Name = "Miss Paris",
    CityOfBirth = nyork
};

context.Cities.Add(nyork);
context.Cities.Add(paris);

context.Persons.Add(bill);
context.Persons.Add(missHilton);

context.SaveChanges();

Console.WriteLine(string.Join(", ", context.Persons.DeepSearch(context.Persons, "Paris")));

Console.ReadLine();