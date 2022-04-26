using System;
using System.Collections.Generic;
using System.Linq;

namespace SO_39748090;

public class Model
{
    public Model()
    {
        Items = Enumerable.Range(0, 10).Select(_ => RandomEx.RandomItemModel()).ToList();
    }

    public IReadOnlyList<ItemModel> Items { get; }
}

public class ItemModel
{
    public ItemModel(string name, IReadOnlyList<StepModel> steps)
    {
        Name = name;
        Duration = TimeSpan.FromTicks(steps.Sum(s => s.Duration.Ticks));
        Steps = steps;
    }

    public string Name { get; }

    public TimeSpan Duration { get; }

    public IReadOnlyList<StepModel> Steps { get; }
}

public class StepModel
{
    public StepModel(TimeSpan duration,
                     Temperature start,
                     Temperature stop)
    {
        Duration = duration;
        Start = start;
        Stop = stop;
    }

    public TimeSpan Duration { get; }
    public Temperature Start { get; }
    public Temperature Stop { get; }
}

public readonly record struct Temperature(double InDegree)
{
    // \u00A0 is NBSP
    public override string ToString() => $"{InDegree:F2}\u00A0°C";
}

public static class RandomEx
{
    private static IReadOnlyList<char> Chars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToList();

    public static T RandomElement<T>(this IReadOnlyList<T> collection)
    {
        return collection[Random.Shared.Next(collection.Count)];
    }

    public static ItemModel RandomItemModel()
    {
        return new ItemModel(RandomString(8),
                             Enumerable.Range(0, 1 + Random.Shared.Next(5))
                                       .Select(_ => RandomStepModel())
                                       .ToList());

    }

    private static StepModel RandomStepModel()
    {
        return new StepModel(RandomTimeSpan(TimeSpan.FromHours(8)),
                             RandomTemperature(new Temperature(250)),
                             RandomTemperature(new Temperature(400)));
    }

    private static Temperature RandomTemperature(Temperature maxTemperature)
    {
        return new Temperature(Random.Shared.NextDouble() * maxTemperature.InDegree);
    }

    public static string RandomString(int length)
    {
        return new string(Enumerable.Range(0, length)
                                    .Select(_ => Chars.RandomElement())
                                    .ToArray());
    }

    private static TimeSpan RandomTimeSpan(TimeSpan maxLength)
    {
        return new TimeSpan(Random.Shared.NextInt64(maxLength.Ticks));
    }
}