using Garyon.Extensions;
using Garyon.Objects;
using Spectre.Console;
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

            table.AddRow(["[yellow]Database[/]", ExecutionTimeMarkup(report.ConnectionTime, report.EnsureDatabaseExistsTime)]);
            table.AddRow(["├── [olive]Connection[/]", ExecutionTimeMarkup(report.ConnectionTime)]);
            table.AddRow(["└── [olive]Ensure Exists[/]", ExecutionTimeMarkup(report.EnsureDatabaseExistsTime)]);
            table.AddRow(["", ""]);
            table.AddRow(["[yellow]Schema[/]", ExecutionTimeMarkup(report.DeleteSchemaTime, report.ConstructSchemaTime)]);
            table.AddRow(["├── [olive]Delete[/]", ExecutionTimeMarkup(report.DeleteSchemaTime)]);
            table.AddRow(["├── [olive]Load File[/]", ExecutionTimeMarkup(report.LoadSchemaFileTime)]);
            table.AddRow(["└── [olive]Construct[/]", ExecutionTimeMarkup(report.ConstructSchemaTime)]);
            table.AddRow(["", ""]);
            table.AddRow(["[yellow]Input[/]", ExecutionTimeMarkup(report.LoadInputFileTime, report.InputTime)]);
            table.AddRow(["├── [olive]Load File[/]", ExecutionTimeMarkup(report.LoadInputFileTime)]);
            table.AddRow(["└── [olive]Run[/]", ExecutionTimeMarkup(report.InputTime)]);
            table.AddRow(["", ""]);
            table.AddRow(["[yellow]Solve[/]", ExecutionTimeMarkup(report.LoadSolveFileTime, report.SolveTime)]);
            table.AddRow(["├── [olive]Load File[/]", ExecutionTimeMarkup(report.LoadSolveFileTime)]);
            table.AddRow(["└── [olive]Run[/]", ExecutionTimeMarkup(report.SolveTime)]);
        }

        public static LiveReportTable New(ChallengeRunReport report)
        {
            var table = ConstructTableForReport(report);
            return new(report, table);
        }
    }

    private static void WriteRunResult(ChallengeRunReport report)
    {
        var table = report.Result!.ConstructSpectreTable()
            .WithPadder()
            .PadLeft(3);
        _console.Write(table);
        _console.WriteLine();
    }

    private static string ExecutionTimeMarkup(
        TimeDuration startingDuration, TimeDuration finalDuration)
    {
        var timeSpan = finalDuration.FromOtherStartingDuration(startingDuration);
        if (timeSpan < TimeSpan.Zero)
        {
            return string.Empty;
        }

        return ExecutionTimeMarkup(timeSpan);
    }

    private static string ExecutionTimeMarkup(TimeDuration duration)
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
        if (time.Seconds > 1.2)
        {
            return $"[red]{time.TotalSeconds:N2} s[/]";
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
