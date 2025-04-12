using Dapper;
using System.Data.Common;
using System.Dynamic;

namespace AdventOfSql.Infrastructure;

public static class DbReaderExtensions
{
    public static async Task<IEnumerable<dynamic>> ExecuteStatementsThenQuery(
        this DbConnection connection, string sql)
    {
        var queries = sql.SplitQueries();

        foreach (var query in queries.SkipLast(1))
        {
            await connection.ExecuteAsync(query);
        }

        var finalSql = queries.Last();
        return await connection.QueryAsync(finalSql);
    }

    public static async Task ExecuteSplitQueries(
        this DbConnection connection,
        string sql)
    {
        var queries = sql.SplitQueries();

        foreach (var query in queries)
        {
            await connection.ExecuteAsync(query);
        }
    }

    public static async IAsyncEnumerable<dynamic> ExecuteParseRowsAsync(
        this DbConnection connection, string sql)
    {
        var reader = await connection.ExecuteReaderAsync(sql);
        await foreach (var row in reader.ParseRows())
        {
            yield return row;
        }
    }

    public static async IAsyncEnumerable<dynamic> ParseRows(
        this DbDataReader reader)
    {
        var schema = await reader.GetColumnSchemaAsync();

        while (true)
        {
            var read = await reader.ReadAsync();
            if (!read)
            {
                yield break;
            }

            dynamic result = new ExpandoObject();
            for (int i = 0; i < schema.Count; i++)
            {
                var value = reader.GetValue(i);
                result[schema[i].ColumnName] = value;
            }

            yield return schema;
        }
    }
}
