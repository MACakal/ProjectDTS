using Neo4j.Driver;

namespace ProjectDTS;

public class GraphDatabaseService
{
    public IDriver Driver { get; }

    public GraphDatabaseService()
    {
        Driver = GraphDatabase.Driver(
            "bolt://localhost:7687",
            AuthTokens.Basic(
                "neo4j",
                "neo4j1234"
            )
        );
    }
}