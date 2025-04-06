using Spectre.Console;
using Spectre.Console.Rendering;

namespace AdventOfSql.Infrastructure;

public static class SpectreExtensions
{
    public static Padder WithPadder(this IRenderable renderable)
    {
        return new(renderable);
    }
}
