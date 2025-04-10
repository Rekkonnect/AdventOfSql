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

        runReport.ConnectionTime.SetStartNow();
        await using var connection = await ConnectionHelpers.CreateOpenConnection();
        runReport.ConnectionTime.SetCurrentDuration();

        // This tracking system can be more sophisticated and generalized

        using (runReport.EnsureDatabaseExistsTime.TrackDuration())
        {
            var databaseName = DatabaseNameForIdentifier(identifier);
            await connection.CreateDatabaseIfNotExists(databaseName);
            await connection.ChangeDatabaseAsync(databaseName);
        }

        using (runReport.DeleteSchemaTime.TrackDuration())
        {
            await connection.DeleteSchema();
        }

        // Schema
        FileInfo schemaFile;
        using (runReport.LoadSchemaFileTime.TrackDuration())
        {
            schemaFile = FileForChallenge(SqlKinds.Schemas, identifier);
        }

        using (runReport.ConstructSchemaTime.TrackDuration())
        {
            await ExecuteSql(connection, schemaFile);
        }

        // Input
        FileInfo inputFile;
        using (runReport.LoadInputFileTime.TrackDuration())
        {
            inputFile = FileForChallenge(SqlKinds.Inputs, identifier);
        }

        using (runReport.InputTime.TrackDuration())
        {
            await ExecuteInput(connection, inputFile);
        }

        // Solution
        FileInfo solutionFile;
        using (runReport.LoadSolveFileTime.TrackDuration())
        {
            solutionFile = FileForChallenge(SqlKinds.Solutions, identifier);
        }

        using (runReport.SolveTime.TrackDuration())
        {
            var solutionResult = await RunSolver(connection, solutionFile);
            runReport.Result = solutionResult;
        }
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
