using Garyon.Objects;
using Spectre.Console;
using System.Diagnostics;

namespace AdventOfSql.Infrastructure;

public sealed class ConsoleChallengeRunner
{
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
        AnsiConsole.MarkupLine($"Running Challenge {challengeIdentifierDisplay}");

        var report = new ChallengeRunReport(identifier);
        var table = LiveReportTable.New(report);

        const double refreshHz = 240;
        const double updateMs = 1000 / refreshHz;
        var displayTable = table.Table
            .Border(TableBorder.Simple)
            .WithPadder()
            .PadLeft(3)
            ;

        var liveTask = AnsiConsole.Live(displayTable)
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
                        .Centered(),
                    new TableColumn($"[cyan]Time[/]")
                        .RightAligned()
                        .Width(18))
                ;
        }

        private static void SetReportValues(
            SpectreTable table, ChallengeRunReport report)
        {
            table.Rows.Clear();
            table.AddRow(["Schema", PrintExecutionTime(report.SchemaTime)]);
            table.AddRow(["Input", PrintExecutionTime(report.InputTime)]);
            table.AddRow(["Solve", PrintExecutionTime(report.SolveTime)]);
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
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static string PrintExecutionTime(TimeDuration? duration)
    {
        if (duration is null)
        {
            return string.Empty;
        }

        var time = duration.FinishedOrCurrentDuration();

        if (time.Seconds > 1)
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
