using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace Kopier.Service;

internal sealed class MySqlService : ServiceBase
{
    internal MySqlService(string connectionString) => DataAdapter = new MySqlDataAdapter(null, connectionString);

    public override bool TransferData(List<DataRow> rows, string targetTableName)
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

        return ExecuteNonQuery(sql.Remove(sql.Length - 1, 1).ToString());
    }
}