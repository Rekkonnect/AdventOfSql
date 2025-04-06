using AdventOfSql.SqlHelpers;
using Dapper;
using Garyon.Extensions;
using Microsoft.Data.SqlClient;

namespace AdventOfSql.Infrastructure;

public sealed class ChallengeRunner
{
    public async Task Run(ChallengeRunReport runReport)
    {
        var identifier = runReport.Identifier;

        await using var connection = await ConnectionHelpers.CreateOpenConnection();

        var databaseName = DatabaseNameForIdentifier(identifier);
        await connection.CreateDatabaseIfNotExists(databaseName);
        await connection.ChangeDatabaseAsync(databaseName);

        await connection.DeleteSchema();

        // Schema
        var schemaFile = FileForChallenge(SqlKinds.Schemas, identifier);

        runReport.SchemaTime = TimeDuration.ForNow();
        await ExecuteSql(connection, schemaFile);
        runReport.SchemaTime.SetCurrentDuration();

        await connection.DeleteAllRows();

        // Input
        var inputFile = FileForChallenge(SqlKinds.Inputs, identifier);

        runReport.InputTime = TimeDuration.ForNow();
        await ExecuteInput(connection, inputFile);
        runReport.InputTime.SetCurrentDuration();

        // Solution
        var solutionFile = FileForChallenge(SqlKinds.Solutions, identifier);

        runReport.SolveTime = TimeDuration.ForNow();
        var solutionResult = await RunSolver(connection, solutionFile);
        runReport.SolveTime.SetCurrentDuration();

        runReport.Result = solutionResult;
    }

    private static async Task ExecuteInput(
        SqlConnection connection,
        FileInfo sqlFile)
    {
        var sql = await sqlFile.ReadAllTextAsync();
        // Pre-process the input to allow more than 1k inserts per command
        // We want this to throw if it fails
        sql = SqlCommandExtensions.BreakDownInsertStatements(sql);
        await connection.ExecuteAsync(sql);
    }

    private static async Task ExecuteSql(
        SqlConnection connection,
        FileInfo sqlFile)
    {
        var sql = await sqlFile.ReadAllTextAsync();
        await connection.ExecuteAsync(sql);
    }

    private static async Task<DapperResult> RunSolver(
        SqlConnection connection,
        FileInfo solutionFile)
    {
        var sql = await solutionFile.ReadAllTextAsync();
        var readRows = await connection.ExecuteStatementsThenQuery(sql);
        var list = readRows.ToList();
        return DapperResult.FromOutput(list);
    }

    private static FileInfo FileForChallenge(
        string sqlKind,
        ChallengeIdentifier identifier)
    {
        return new(DetermineFileNameForChallenge(sqlKind, identifier));
    }

    private static string DetermineFileNameForChallenge(
        string sqlKind,
        ChallengeIdentifier identifier)
    {
        identifier = IdentifierForSqlFileKind(sqlKind, identifier);
        return $"Challenges/{sqlKind}/Year{identifier.DayIdentifier.Year}/{identifier.InputFileName}.sql";
    }

    private static ChallengeIdentifier IdentifierForSqlFileKind(
        string sqlKind,
        ChallengeIdentifier identifier)
    {
        switch (sqlKind)
        {
            case SqlKinds.Inputs:
                return identifier;

            default:
                return identifier.WithRealInputIdentifier();
        }
    }

    private static string DatabaseNameForIdentifier(ChallengeIdentifier identifier)
    {
        var (year, day) = identifier.DayIdentifier;
        return $"AdventOfSql_Year{year}_Day{day}";
    }

    private static class SqlKinds
    {
        public const string Inputs = "Inputs";
        public const string Schemas = "Schemas";
        public const string Solutions = "Solutions";
    }
}
