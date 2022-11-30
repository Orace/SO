using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

class Solution
{
    static void Main(string[] args)
    {
        var taskCount = int.Parse(Console.ReadLine()!);
        var tasks = Enumerable.Range(0, taskCount)
                              .Select(_ => Console.ReadLine()!.Split(' ').Select(int.Parse).ToArray())
                              .OrderBy(t => t[0])
                              .Select((a, i) => new Task
                               {
                                   Index = i,
                                   StartDay = a[0],
                                   Duration = a[1],
                                   OverlapCount = 0
                               })
                              .ToArray();


        for (var i = 0; i < taskCount; i++)
        {
            var task = tasks[i];

            for (var j = i + 1; j < taskCount; j++)
            {
                var otherTask = tasks[j];
                if (task.StartDay + task.Duration <= otherTask.StartDay)
                    break;

                otherTask.OverlapCount++;
                task.OverlapCount++;
            }
        }

        var sortedTasks = new SortedSet<Task>(tasks, new TaskComparer());

        while (sortedTasks.Count > 0)
        {
            var task = sortedTasks.Last();

            var i = task.Index;
            for (var j = i - 1; j >= 0; j--)
            {
                var otherTask = tasks[j];
                if (otherTask.StartDay + otherTask.Duration <= task.StartDay)
                    break;

                sortedTasks.Remove(otherTask);
                otherTask.OverlapCount--;

                if (otherTask.OverlapCount > 0)
                {
                    sortedTasks.Add(otherTask);
                }
            }

            for (var j = i + 1; j < taskCount; j++)
            {
                var otherTask = tasks[j];
                if (task.StartDay + task.Duration <= otherTask.StartDay)
                    break;

                sortedTasks.Remove(otherTask);
                otherTask.OverlapCount--;

                if (otherTask.OverlapCount > 0)
                {
                    sortedTasks.Add(otherTask);
                }
            }

            sortedTasks.Remove(task);
            task.Duration = 0;
        }

        Console.WriteLine(tasks.Count(t => t.Duration > 0));
    }
}

internal class TaskComparer : IComparer<Task>
{
    public int Compare(Task x, Task y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        var overlapCountComparison = x.OverlapCount.CompareTo(y.OverlapCount);
        if (overlapCountComparison != 0) return overlapCountComparison;
        return x.Index.CompareTo(y.Index);
    }
}

class Task
{
    public int Index { get; set; }
    public int StartDay { get; set; }
    public int Duration { get; set; }
    public int OverlapCount { get; set; }
}