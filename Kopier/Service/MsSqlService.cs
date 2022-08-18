using System.Data;
using System.Data.SqlClient;

namespace Kopier.Service;

internal sealed class MsSqlService : ServiceBase
{
    internal MsSqlService(string connectionString) => DataAdapter = new SqlDataAdapter(null, connectionString);

    public override async Task<int> GetCountAsync(string query)
    {
        var result = await ExecuteScalarAsync($"SELECT COUNT(*) FROM ({query}) TBX");
        return result.ToInt();
    }

    public override async Task<DataTable> GetDataAsync(string query, string keyFields, int skip, int take)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentNullException(nameof(query));
        }

        if (!string.IsNullOrWhiteSpace(keyFields) && take > 0)
        {
            query = $"SELECT * FROM (SELECT X.*, RANK() OVER (ORDER BY {keyFields}) RNK FROM ({query}) X) TBX WHERE RNK BETWEEN {skip + 1} AND {skip + take}";
        }

        var result = await FillDataTableAsync(query);
        if (result.Columns["RNK"] is not null)
        {
            result.Columns.Remove("RNK");
        }
        return result;
    }

    public override Task<bool> TransferDataAsync(List<DataRow> rows, string targetTableName) => throw new NotImplementedException();
}