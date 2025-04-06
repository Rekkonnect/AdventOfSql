using AdventOfSql.Infrastructure;
using Garyon.Objects;

await Singleton<ConsoleChallengeRunner>.Instance
    .RunMany([
        new(2024, 01),
        new(2024, 02),
        new(2024, 03),
        new(2024, 05),
        new(2024, 06),
        new(2024, 07),
        new(2024, 08),
        new(2024, 09),
        new(2024, 10),
    ]);
