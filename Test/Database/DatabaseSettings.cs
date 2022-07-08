namespace Test.Database;

public class DatabaseSettings
{

    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public List<string> CollectionName { get; set; } = new List<string>();

}
