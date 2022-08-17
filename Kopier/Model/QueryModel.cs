namespace Kopier.Model;

public class QueryModel
{
    public string Name { get; set; }
    public string Query { get; set; }
    public ConnectionModel Source { get; set; }
    public ConnectionModel Target { get; set; }
    public string Database { get; set; }
    public string TableName { get; set; }
}