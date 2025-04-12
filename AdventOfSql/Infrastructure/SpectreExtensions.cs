using Spectre.Console;
using Spectre.Console.Rendering;

namespace AdventOfSql.Infrastructure;

public static class SpectreExtensions
{
    public static Padder WithPadder(this IRenderable renderable)
    {
        return new(renderable);
    }

    public static void WriteLine(
        this IAnsiConsole console,
        IRenderable renderable)
    {
        console.Write(renderable);
        console.WriteLine();
    }

    public static void Write(
        this IRenderable renderable,
        IAnsiConsole? console = null)
    {
        console ??= AnsiConsole.Console;
        console.Write(renderable);
    }

    public static void WriteLine(
        this IRenderable renderable,
        IAnsiConsole? console = null)
    {
        console ??= AnsiConsole.Console;
        console.WriteLine(renderable);
    }
}
