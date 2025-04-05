using Garyon.Objects;
using Spectre.Console;

namespace AdventOfSql.Infrastructure;

public sealed class ConsoleChallengeRunner
{
    private readonly ChallengeRunner _challengeRunner = Singleton<ChallengeRunner>.Instance;

    public async Task Run(ChallengeIdentifier identifier)
    {
        var challengeIdentifierDisplay = FormatChallengeIdentifier(identifier);
        AnsiConsole.MarkupLine($"Running Challenge {challengeIdentifierDisplay}\n");

        var result = await _challengeRunner.Run(identifier);
        WriteRunResult(result);
    }

    private static void WriteRunResult(ChallengeRunResult result)
    {
        AnsiConsole.MarkupLine($"""
               Schema time   {PrintExecutionTime(result.SchemaTime)}
                Input time   {PrintExecutionTime(result.InputTime)}
                Solve time   {PrintExecutionTime(result.SolveTime)}

            Result Table
            

            """);

        var table = result.Result.ConstructSpectreTable();
        AnsiConsole.Write(table);
    }

    private static string PrintExecutionTime(TimeSpan time)
    {
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
