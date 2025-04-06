using AdventOfSql.Infrastructure;
using Garyon.Objects;

await Singleton<ConsoleChallengeRunner>.Instance
    .RunMany([
        new(2024, 09),
    ]);
