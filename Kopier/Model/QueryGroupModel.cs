namespace Kopier.Model;

public class QueryGroupModel : List<QueryModel>
{
    public string Name { get; private set; }

    public QueryGroupModel(string name, List<QueryModel> queries) : base(queries) => Name = name;
}