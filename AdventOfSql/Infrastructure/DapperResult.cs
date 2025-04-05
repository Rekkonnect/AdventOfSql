using System.Collections.Immutable;

namespace AdventOfSql.Infrastructure;

public sealed record DapperResult(
    QueryOutput Result,
    IReadOnlyList<string> ColumnNames,
    IReadOnlyList<IReadOnlyList<object>> Rows)
{
    public int ValueCount => Rows.Count;

    private static DapperResult FromEmpty(QueryOutput output)
    {
        return new(output, [], []);
    }

    public static DapperResult FromOutput(QueryOutput output)
    {
        if (output is [])
        {
            return FromEmpty(output);
        }

        var firstValue = output.First()!;
        var propertyDictionary = (IDictionary<string, object>)firstValue;
        var columnNames = propertyDictionary.Keys.ToList();
        var rowValues = ImmutableArray
            .CreateBuilder<IReadOnlyList<object>>(output.Count);
        foreach (IDictionary<string, object> row in output)
        {
            var values = row.Values.ToList();
            rowValues.Add(values);
        }
        return new(output, columnNames, rowValues.ToImmutable());
    }
}
