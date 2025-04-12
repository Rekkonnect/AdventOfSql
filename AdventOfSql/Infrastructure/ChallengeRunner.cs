using AdventOfSql.SqlHelpers;
using Dapper;
using Garyon.Extensions;
using Microsoft.Data.SqlClient;

namespace AdventOfSql.Infrastructure;

public sealed class ChallengeRunner
{
    public async Task Run(ChallengeRunReport runReport)
    {
        try
        {
            await RunCore(runReport);
        }
        catch (Exception ex)
        {
            runReport.Exception = ex;
        }
    }

    private static async Task RunCore(ChallengeRunReport report)
    {
        var identifier = report.Identifier;
        var runFlow = report.RunFlow;
        var stepFlowManager = new StepFlowManager();

        stepFlowManager.BeginTracking(runFlow.ConnectionTime);
        await using var connection = await ConnectionHelpers.CreateOpenConnection();

        // This tracking system can be more sophisticated and generalized

        stepFlowManager.BeginTracking(runFlow.EnsureDatabaseExistsTime);
        var databaseName = DatabaseNameForIdentifier(identifier);
        await connection.CreateDatabaseIfNotExists(databaseName);
        await connection.ChangeDatabaseAsync(databaseName);

        stepFlowManager.BeginTracking(runFlow.DeleteSchemaTime);
        await connection.DeleteSchema();

        // Supplementary
        stepFlowManager.BeginTracking(runFlow.LoadSupplementaryFileTime);
        string? supplementarySql = null;
        var supplementaryFile = FileForChallenge(SqlKinds.Supplementary, identifier);
        try
        {
            supplementarySql = await supplementaryFile.ReadAllTextAsync();
        }
        catch (FileNotFoundException)
        {
            // If the file was not found; we ignore the issue and move on
        }

        stepFlowManager.BeginTracking(runFlow.ConstructSupplementaryTime);
        if (supplementarySql is not null)
        {
            await connection.ExecuteSplitQueries(supplementarySql);
        }

        // Schema
        stepFlowManager.BeginTracking(runFlow.LoadSchemaFileTime);
        var schemaFile = FileForChallenge(SqlKinds.Schemas, identifier);
        var schemaSql = await schemaFile.ReadAllTextAsync();

        stepFlowManager.BeginTracking(runFlow.ConstructSchemaTime);
        await connection.ExecuteSplitQueries(schemaSql);

        // Input
        stepFlowManager.BeginTracking(runFlow.LoadInputFileTime);
        var inputFile = FileForChallenge(SqlKinds.Inputs, identifier);
        var inputSql = await inputFile.ReadAllTextAsync();

        stepFlowManager.BeginTracking(runFlow.InputTime);
        await ExecuteInput(connection, inputSql);

        // Solution
        stepFlowManager.BeginTracking(runFlow.LoadSolveFileTime);
        var solutionFile = FileForChallenge(SqlKinds.Solutions, identifier);
        var solutionSql = await solutionFile.ReadAllTextAsync();

        stepFlowManager.BeginTracking(runFlow.SolveTime);
        var solutionResult = await RunSolver(connection, solutionSql);
        report.Result = solutionResult;

        stepFlowManager.Finish();
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
