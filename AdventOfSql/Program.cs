using AdventOfSql.Infrastructure;
using Garyon.Objects;

var identifier = new ChallengeIdentifier(2024, 02, TestCaseIdentifier.RealInput);
await Singleton<ConsoleChallengeRunner>.Instance.Run(identifier);
