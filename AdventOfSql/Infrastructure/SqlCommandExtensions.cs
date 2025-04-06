using Garyon.Objects;
using SqlParser;
using SqlParser.Ast;
using SqlParser.Dialects;

namespace AdventOfSql.Infrastructure;

public static class SqlCommandExtensions
{
    public const int MsSqlInsertRowLimit = 1000;

    public static string BreakDownInsertStatements(
        string sql, int limit = MsSqlInsertRowLimit)
    {
        var parser = Singleton<SqlQueryParser>.Instance;
        var dialect = Singleton<MsSqlDialect>.Instance;
        var ast = parser.Parse(sql, dialect);
        var replaced = new Sequence<Statement>(
            ast.SelectMany(ReplaceStatements));
        return SqlWritingExtensions.ToSqlDelimited(replaced, $";{Environment.NewLine}");

        IReadOnlyList<Statement> ReplaceStatements(Statement statement)
        {
            if (statement is Statement.Insert insert)
            {
                var source = insert.InsertOperation.Source?.AsSelect();
                var bodyValues = source?.Query.Body
                    .As<SetExpression.ValuesExpression>();
                if (bodyValues is null)
                {
                    return [statement];
                }

                var values = bodyValues.Values;
                if (values.Rows.Count > limit)
                {
                    // Well this is clunky
                    var inserts = values.Rows.Chunk(limit)
                        .Select(chunk => insert with
                        {
                            InsertOperation = insert.InsertOperation with
                            {
                                Source = source! with
                                {
                                    Query = source.Query with
                                    {
                                        Body = bodyValues with
                                        {
                                            Values = new(chunk),
                                        }
                                    }
                                }
                            }
                        })
                        .ToList()
                        ;
                    return inserts;
                }
            }

            return [statement];
        }
    }
}
