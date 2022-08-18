using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace Kopier.Service;

internal sealed class MySqlService : ServiceBase
{
    internal MySqlService(string connectionString) => DataAdapter = new MySqlDataAdapter(null, connectionString);

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
            query = $"{query} ORDER BY {keyFields} LIMIT {skip + 1}, {take}";
        }

        return await FillDataTableAsync(query);
    }

    public override async Task<bool> TransferDataAsync(List<DataRow> rows, string targetTableName)
    {
        var columns = rows.FirstOrDefault().Table.Columns.Cast<DataColumn>().ToList();
        var sql = new StringBuilder("SET foreign_key_checks = 0;\r\n\r\n").Append("Insert Into ").Append(targetTableName).Append(" (");
        sql.Append(columns.Aggregate("", (cur, clm) => cur + (clm.Caption + ", "))).Remove(sql.Length - 2, 2).Append(") Values ");

        DataAdapter.SelectCommand.Parameters.Clear();

        for (var i = 0; i < rows.Count; i++)
        {
            sql.AppendLine().Append("\t(");

            for (var n = 0; n < columns.Count; n++)
            {
                sql.Append("?prm").Append(columns[n].Caption).Append(i).Append(", ");
                DataAdapter.SelectCommand.Parameters.Add(new MySqlParameter($"?prm{columns[n].Caption}{i}", rows[i][n]));
            }

            sql.Remove(sql.Length - 2, 2).Append("),");
        }

        return await ExecuteNonQueryAsync(sql.Remove(sql.Length - 1, 1).ToString());
    }
}