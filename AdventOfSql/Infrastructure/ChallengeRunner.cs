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
        var timeDurationFlowManager = new TimeDurationFlowManager();

        timeDurationFlowManager.BeginTracking(runReport.ConnectionTime);
        await using var connection = await ConnectionHelpers.CreateOpenConnection();

        // This tracking system can be more sophisticated and generalized

        timeDurationFlowManager.BeginTracking(runReport.EnsureDatabaseExistsTime);
        var databaseName = DatabaseNameForIdentifier(identifier);
        await connection.CreateDatabaseIfNotExists(databaseName);
        await connection.ChangeDatabaseAsync(databaseName);

        timeDurationFlowManager.BeginTracking(runReport.DeleteSchemaTime);
        await connection.DeleteSchema();

        // Supplementary
        string? supplementarySql = null;
        timeDurationFlowManager.BeginTracking(runReport.LoadSupplementaryFileTime);
        var supplementaryFile = FileForChallenge(SqlKinds.Supplementary, identifier);
        try
        {
            supplementarySql = await supplementaryFile.ReadAllTextAsync();
        }
        catch (FileNotFoundException)
        {
            // If the file was not found; we ignore the issue and move on
        }

        timeDurationFlowManager.BeginTracking(runReport.ConstructSupplementaryTime);
        if (supplementarySql is not null)
        {
            await connection.ExecuteSplitQueries(supplementarySql);
        }

        // Schema
        timeDurationFlowManager.BeginTracking(runReport.LoadSchemaFileTime);
        var schemaFile = FileForChallenge(SqlKinds.Schemas, identifier);
        var schemaSql = await schemaFile.ReadAllTextAsync();

        timeDurationFlowManager.BeginTracking(runReport.ConstructSchemaTime);
        await connection.ExecuteSplitQueries(schemaSql);

        // Input
        timeDurationFlowManager.BeginTracking(runReport.LoadInputFileTime);
        var inputFile = FileForChallenge(SqlKinds.Inputs, identifier);
        var inputSql = await inputFile.ReadAllTextAsync();

        timeDurationFlowManager.BeginTracking(runReport.InputTime);
        await ExecuteInput(connection, inputSql);

        // Solution
        timeDurationFlowManager.BeginTracking(runReport.LoadSolveFileTime);
        var solutionFile = FileForChallenge(SqlKinds.Solutions, identifier);
        var solutionSql = await solutionFile.ReadAllTextAsync();

        timeDurationFlowManager.BeginTracking(runReport.SolveTime);
        var solutionResult = await RunSolver(connection, solutionSql);
        runReport.Result = solutionResult;

        timeDurationFlowManager.Finish();
    }

    private static async Task ExecuteInput(
        SqlConnection connection,
        string sql)
    {
        // Pre-process the input to allow more than 1k inserts per command
        // We want this to throw if it fails
        sql = SqlCommandExtensions.BreakDownInsertStatements(sql);
        await connection.ExecuteAsync(sql);
    }

    private static async Task<DapperResult> RunSolver(
        SqlConnection connection,
        string sql)
    {
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
        public const string Supplementary = "Supplementary";
    }
}
