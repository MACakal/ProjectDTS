namespace UnitTests;

[TestClass]
public class RedisServerDatabaseStoryTests
{
    [TestMethod]
    public void RedisDatabase_ShouldUseEnvironmentConnectionString_InProgram()
    {
        var programText = File.ReadAllText(GetProjectFile("Program.cs"));

        StringAssert.Contains(programText, "Env.GetString(\"REDIS_URL\")");
    }

    [TestMethod]
    public void RedisDatabase_ShouldNotUseDefaultLocalhostPort_InRedisCode()
    {
        var redisFiles = Directory.GetFiles(GetProjectRoot(), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}UnitTests{Path.DirectorySeparatorChar}"))
            .Where(path => File.ReadAllText(path).Contains("Redis", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.IsTrue(redisFiles.Count > 0);
        foreach (var file in redisFiles)
        {
            var text = File.ReadAllText(file);
            Assert.IsFalse(text.Contains("localhost:6379", StringComparison.OrdinalIgnoreCase), file);
            Assert.IsFalse(text.Contains("127.0.0.1:6379", StringComparison.OrdinalIgnoreCase), file);
        }
    }

    private static string GetProjectFile(params string[] parts)
    {
        return Path.Combine(new[] { GetProjectRoot() }.Concat(parts).ToArray());
    }

    private static string GetProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "ProjectDTS.csproj")))
        {
            directory = directory.Parent;
        }

        Assert.IsNotNull(directory, "Could not find project root.");
        return directory!.FullName;
    }
}
