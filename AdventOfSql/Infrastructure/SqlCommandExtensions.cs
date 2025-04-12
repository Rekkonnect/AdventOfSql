using Garyon.Objects;
using SqlParser;
using SqlParser.Ast;
using SqlParser.Dialects;
using System.Text.RegularExpressions;

namespace AdventOfSql.Infrastructure;

public static class SqlCommandExtensions
{
    public const int MsSqlInsertRowLimit = 1000;

    public static string BreakDownInsertStatements(
        string sql, int limit = MsSqlInsertRowLimit)
    {
        var parser = Singleton<SqlQueryParser>.Instance;
        var dialect = Singleton<MsSqlDialect>.Instance;
        Sequence<Statement> ast;
        try
        {
            ast = parser.Parse(sql, dialect);
        }
        catch (ParserException)
        {
            // The parser may fail in known cases like this:
            // - `GEOGRAPHY::Point(-73.985130, 40.758896, 4326)` fails
            //     The `-` before 73.98... is not supported by the parser
            return sql;
        }

        var replacements = ast.Select(ReplaceStatements).ToList();
        if (replacements.All(static s => s.IsNoOp))
        {
            // Avoid changing the SQL -- the parser has some serious bugs
            // that need be worked out
            // Notable example:
            // - `GEOGRAPHY::STPolyFromText('POLYGON((-74.25909 40.477399, ...` breaks
            //     The parser does not respect the string argument and removes the quotes,
            //     reconstructing the above as:
            //     `GEOGRAPHY::STPolyFromText(POLYGON((-74.25909 40.477399, ...`
            return sql;
        }
        var replaced = new Sequence<Statement>(
            replacements.SelectMany(static s => s.ReplacedOrOriginal));
        return SqlWritingExtensions.ToSqlDelimited(replaced, $";{Environment.NewLine}");

        StatementReplacement ReplaceStatements(Statement statement)
        {
            if (statement is Statement.Insert insert)
            {
                var source = insert.InsertOperation.Source?.AsSelect();
                var bodyValues = source?.Query.Body
                    .As<SetExpression.ValuesExpression>();
                if (bodyValues is null)
                {
                    return StatementReplacement.Empty(statement);
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
                    return new(statement, inserts);
                }
            }

            return StatementReplacement.Empty(statement);
        }
    }

    private sealed record StatementReplacement(
        Statement Original,
        IReadOnlyList<Statement> Replacements)
    {
        public IReadOnlyList<Statement> ReplacedOrOriginal
        {
            get
            {
                if (IsNoOp)
                {
                    return [Original];
                }

                return Replacements;
            }
        }

        public bool IsNoOp => Replacements is [];

        public static StatementReplacement Empty(Statement original)
        {
            return new(original, []);
        }
    }

    public static IReadOnlyList<string> SplitQueries(this string sql)
    {
        return Regex.Split(sql, @"\bGO\b")
            .Where(s => s.AsSpan().Trim() is not "")
            .ToList();
    }
}
