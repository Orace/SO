using System;
using System.Collections.Generic;
using System.Linq;

namespace SO_39748090;

public class FlatViewModel
{
    public FlatViewModel(Model model)
    {
        Items = model.Items
                     .SelectMany(item => item.Steps.Select((step, i) => new FlatItemViewModel(item, i, step.Duration, step.Start, step.Stop)))
                     .ToList();
    }

    public IReadOnlyList<FlatItemViewModel> Items { get; }
}

public class FlatItemViewModel
{
    public FlatItemViewModel(ItemModel item,
                             int stepIndex,
                             TimeSpan stepDuration,
                             Temperature stepStart,
                             Temperature stepStop)
    {
        GroupKey = item;
        Name = item.Name;
        TotalDuration = item.Duration;
        StepPosition = $"{stepIndex + 1}/{item.Steps.Count}";
        StepDuration = stepDuration;
        StepStart = stepStart;
        StepStop = stepStop;
    }

    public object GroupKey { get; }

    public string Name { get; }

    public TimeSpan TotalDuration { get; }

    public string StepPosition { get; }

    public TimeSpan StepDuration { get; }

    public Temperature StepStart { get; }

    public Temperature StepStop { get; }
}

public class ViewModel
{
    public ViewModel(Model model)
    {
        Items = model.Items.Select(ModelToViewModel).ToList();
    }

    public IReadOnlyList<ItemViewModel> Items { get; }

    private static ItemViewModel ModelToViewModel(ItemModel item)
    {
        return new ItemViewModel(item.Name, item.Steps.Select(ModelToViewModel).ToList());
    }

    private static StepViewModel ModelToViewModel(StepModel stepModel)
    {
        return new StepViewModel(stepModel.Duration, stepModel.Start, stepModel.Stop);
    }
}

public class ItemViewModel
{
    public ItemViewModel(string name, IReadOnlyList<StepViewModel> steps)
    {
        Name = name;
        Duration = TimeSpan.FromTicks(steps.Sum(s => s.Duration.Ticks));
        Steps = steps;
    }

    public string Name { get; }

    public TimeSpan Duration { get; }

    public IReadOnlyList<StepViewModel> Steps { get; }
}

public class StepViewModel
{
    public StepViewModel(TimeSpan duration,
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