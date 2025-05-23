﻿namespace AdventOfSql.Infrastructure;

public static class FileSystemExtensions
{
    public static string ReadAllText(this FileInfo file)
    {
        return File.ReadAllText(file.FullName);
    }

    public static async Task<string> ReadAllTextAsync(this FileInfo file)
    {
        return await File.ReadAllTextAsync(file.FullName);
    }
}
