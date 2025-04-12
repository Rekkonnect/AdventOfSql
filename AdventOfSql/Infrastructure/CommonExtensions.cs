namespace AdventOfSql.Infrastructure;

public static class CommonExtensions
{
    public static bool IsLastIndex(this int index, int length)
    {
        return index == length - 1;
    }
}
