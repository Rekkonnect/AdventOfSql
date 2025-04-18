using Garyon.Objects;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;

namespace AdventOfSql.Infrastructure;

public sealed class ConsoleChallengeRunner
{
    private static readonly IAnsiConsole _console = AnsiConsole.Console;

    private readonly ChallengeRunner _challengeRunner = Singleton<ChallengeRunner>.Instance;

    public async Task RunMany(IReadOnlyList<ChallengeIdentifier> identifiers)
    {
        foreach (var identifier in identifiers)
        {
            await Run(identifier);
        }
    }

    public async Task Run(ChallengeIdentifier identifier)
    {
        var challengeIdentifierDisplay = FormatChallengeIdentifier(identifier);
        _console.MarkupLine($"Running Challenge {challengeIdentifierDisplay}");

        var report = new ChallengeRunReport(identifier);
        var table = LiveReportTable.New(report);

        const double refreshHz = 240;
        const double updateMs = 1000 / refreshHz;
        var displayTable = table.Table
            .Border(TableBorder.Simple)
            .WithPadder()
            .PadLeft(3)
            ;

        var liveTask = _console.Live(displayTable)
            .StartAsync(async context =>
            {
                await Task.Yield();

                while (true)
                {
                    table.Update();
                    context.Refresh();

                    if (report.HasFinished)
                    {
                        return Task.CompletedTask;
                    }

                    Thread.Sleep((int)updateMs);
                }
            });

        await _challengeRunner.Run(report);
        await liveTask;
        WriteRunResult(report);
    }

    private sealed record LiveReportTable(
        ChallengeRunReport Report,
        SpectreTable Table)
    {
        private long _lastUpdateTimestamp;

        public void Update()
        {
            SetReportValues(Table, Report);

            var updateDelay = Stopwatch.GetElapsedTime(_lastUpdateTimestamp);
            var ms = updateDelay.TotalSeconds;
            var fps = 1 / ms;
            Table.Caption($"Update [red]{fps:N2} FPS[/]");
            _lastUpdateTimestamp = Stopwatch.GetTimestamp();
        }

        private static SpectreTable ConstructTableForReport(ChallengeRunReport report)
        {
            return new SpectreTable()
                .AddColumns(
                    new TableColumn($"[cyan]Stage[/]")
                        .LeftAligned(),
                    new TableColumn($"[cyan]Time[/]")
                        .RightAligned()
                        .Width(18))
                ;
        }

        private static void SetReportValues(
            SpectreTable table, ChallengeRunReport report)
        {
            table.Rows.Clear();
            AddStepTrackParents(table, report.RunFlow.Parents);
        }

        private static void AddEmptyRow(SpectreTable table)
        {
            table.AddRow(["", ""]);
        }

        private const string _nestedWithFollowingChildrenPrefix = "├──";
        private const string _nestedFinalChildPrefix = "└──";

        private static string PrefixForIndex(int index, int length)
        {
            var isLast = index.IsLastIndex(length);
            return isLast
                ? _nestedFinalChildPrefix
                : _nestedWithFollowingChildrenPrefix
                ;
        }

        private static void AddStepTrackParents(
            SpectreTable table,
            IReadOnlyList<StepTrackParent> parents)
        {
            int parentsLength = parents.Count;
            for (int i = 0; i < parentsLength; i++)
            {
                var parent = parents[i];
                AddStepTrackParentRows(table, parent);
                if (!i.IsLastIndex(parentsLength))
                {
                    AddEmptyRow(table);
                }
            }
        }

        private static void AddStepTrackParentRows(
            SpectreTable table,
            StepTrackParent parent)
        {
            table.AddRow([$"[yellow]{parent.Label}[/]", ExecutionTimeMarkup(parent)]);
            var tracks = parent.Tracks;
            int trackCount = tracks.Length;
            for (int i = 0; i < trackCount; i++)
            {
                var prefix = PrefixForIndex(i, trackCount);
                var track = tracks[i];
                AddStepTrackRow(table, prefix, track);
            }
        }

        private static void AddStepTrackRow(
            SpectreTable table,
            string prefix,
            StepTrack track)
        {
            table.AddRow([$"{prefix} [olive]{track.Label}[/]", ExecutionTimeMarkup(track)]);
        }

        public static LiveReportTable New(ChallengeRunReport report)
        {
            var table = ConstructTableForReport(report);
            return new(report, table);
        }
    }

    private static void WriteRunResult(ChallengeRunReport report)
    {
        if (report.Exception is not null)
        {
            _console.WriteException(report.Exception);
            return;
        }

        WriteSuccessfulOutput(report);
    }

    private static void WriteSuccessfulOutput(ChallengeRunReport report)
    {
        RunOutputRenderable(report)
            .WithPadder()
            .PadLeft(3)
            .WriteLine(_console);
    }

    private static IRenderable RunOutputRenderable(ChallengeRunReport report)
    {
        var result = report.Result!;
        if (result.Rows.Count is 0)
        {
            return new Markup("[red]No rows were returned -- the solution is wrong[/]");
        }

        return result.ConstructSpectreTable();
    }

    private static string ExecutionTimeMarkup(
        StepTrack startingDuration, StepTrack finalDuration)
    {
        var timeSpan = finalDuration.FromOtherStartingStep(startingDuration);
        if (timeSpan < TimeSpan.Zero)
        {
            return string.Empty;
        }

        return ExecutionTimeMarkup(timeSpan);
    }

    private static string ExecutionTimeMarkup(
        StepTrackParent parent)
    {
        var timeSpan = parent.Elapsed();
        if (timeSpan < TimeSpan.Zero)
        {
            return string.Empty;
        }

        return ExecutionTimeMarkup(timeSpan);
    }

    private static string ExecutionTimeMarkup(StepTrack duration)
    {
        if (duration.IsUninitialized)
        {
            return string.Empty;
        }

        var time = duration.FinishedOrCurrentDuration();
        return ExecutionTimeMarkup(time);
    }

    private static string ExecutionTimeMarkup(TimeSpan time)
    {
        if (time.Microseconds < 1)
        {
            return $"[yellow]---   [/]";
        }
        if (time.Seconds > 1.2)
        {
            return $"[red]{time.TotalSeconds:N2}  s[/]";
        }
        if (time.Milliseconds > 10)
        {
            return $"[green]{time.TotalMilliseconds:N2} ms[/]";
        }
        return $"[blue]{time.TotalMicroseconds:N2} us[/]";
    }

    private static string FormatChallengeIdentifier(ChallengeIdentifier identifier)
    {
        var ChallengeDayDisplay = FormatChallengeDay(identifier.DayIdentifier);
        var testCaseDisplay = FormatTestCase(identifier.TestCaseIdentifier);
        return $"{ChallengeDayDisplay} ({testCaseDisplay})";
    }

    private static string FormatChallengeDay(ChallengeDayIdentifier identifier)
    {
        return $"[teal]Year[/] [cyan]{identifier.Year}[/] - [teal]Day[/] [cyan]{identifier.Day:00}[/]";
    }

    private static string FormatTestCase(TestCaseIdentifier identifier)
    {
        if (identifier.IsTestCase)
        {
            return $"[olive]Test Case[/] [yellow]{identifier.TestCase}[/]";
        }

        return "[green]Real Input[/]";
    }
}
