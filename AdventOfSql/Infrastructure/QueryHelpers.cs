using Spectre.Console;

namespace AdventOfSql.Infrastructure;

public static class QueryHelpers
{
    public static SpectreTable ConstructSpectreTable(this DapperResult output)
    {
        var table = new SpectreTable();

        foreach (var column in output.ColumnNames)
        {
            table.AddColumn(
                new TableColumn($"[cyan]{column}[/]")
                    .Centered());
        }

        foreach (var row in output.Rows)
        {
            var rowStrings = row
                .Select(s => s.ToString()!)
                .ToArray();
            table.AddRow(rowStrings);
        }

        return table;
    }
}
