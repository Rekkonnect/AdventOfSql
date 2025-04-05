using Dapper;
using Microsoft.Data.SqlClient;

namespace AdventOfSql.SqlHelpers;

public static class SqlServerExtensions
{
    public static async Task DeleteAllRows(this SqlConnection database)
    {
        // https://stackoverflow.com/a/1899881
        const string sql = """
            EXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?';
            EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
            EXEC sp_MSForEachTable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?';
            EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
            EXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?';
            """;

        await database.ExecuteAsync(sql);
    }

    public static async Task DeleteSchema(this SqlConnection database)
    {
        const string sql = """
            EXEC sp_MSForEachTable 'DROP TABLE ?';
            """;

        await database.ExecuteAsync(sql);
    }

    public static async Task CreateDatabaseIfNotExists(
        this SqlConnection database,
        string databaseName)
    {
        var sql = $"""
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
            BEGIN
               CREATE DATABASE [{databaseName}];
            END
            """;

        await database.ExecuteAsync(sql);
    }
}
