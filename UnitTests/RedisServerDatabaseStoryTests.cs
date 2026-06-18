namespace UnitTests;

[TestClass]
public class RedisServerDatabaseStoryTests
{
    [DataTestMethod]
    [DataRow("Program.cs")]
    [DataRow("Logic\\ProductLogic.cs")]
    [DataRow("Presentation\\PastOrders.cs")]
    public void RedisDatabase_ShouldUseEnvironmentConnectionString_InRedisCode(string relativePath)
    {
        var text = File.ReadAllText(GetProjectFile(relativePath.Split('\\')));

        StringAssert.Contains(text, "REDIS_URL");
    }

    [DataTestMethod]
    [DataRow("localhost:6379")]
    [DataRow("127.0.0.1:6379")]
    [DataRow("localhost,abortConnect=false")]
    public void RedisDatabase_ShouldNotUseDefaultLocalhostConnection_InRedisCode(string forbiddenConnection)
    {
        var redisFiles = Directory.GetFiles(GetProjectRoot(), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}UnitTests{Path.DirectorySeparatorChar}"))
            .Where(path => File.ReadAllText(path).Contains("Redis", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.IsTrue(redisFiles.Count > 0);
        foreach (var file in redisFiles)
        {
            var text = File.ReadAllText(file);
            Assert.IsFalse(text.Contains(forbiddenConnection, StringComparison.OrdinalIgnoreCase), file);
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
