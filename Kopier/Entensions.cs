using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Linq;
using Newtonsoft.Json;

namespace Kopier;

public static class Entensions
{
    public static bool ToBool(this object val, bool defaultValue = false)
    {
        return val == DBNull.Value || val == null ? defaultValue : bool.Parse(val.ToString());
    }

    public static string ToSql(this IDbCommand command)
    {
        foreach (DbParameter prm in command.Parameters)
        {
            command.CommandText = command.CommandText.Replace($":{prm.ParameterName}", prm.GetParameterValue());
        }
        return command.CommandText;
    }

    private static string GetParameterValue(this DbParameter parameter)
    {
        var ret = parameter.DbType switch
        {
            DbType.Xml or DbType.Time or DbType.Date or DbType.DateTime or DbType.DateTime2 or DbType.DateTimeOffset or
            DbType.String or DbType.AnsiString or DbType.StringFixedLength or DbType.AnsiStringFixedLength
                => $"'{parameter.Value.ToString().Replace("'", "''")}'",
            DbType.Boolean => parameter.Value.ToBool() ? "1" : "0",
            _ => parameter.Value.ToString().Replace("'", "''"),
        };
        return ret == "''" ? "NULL" : ret;
    }

    public static string ToJson(this DataTable table)
    {
        var data = new List<Dictionary<string, object>>();
        foreach (var (row, dict) in from DataRow row in table.Rows
                                    let dict = new Dictionary<string, object>()
                                    select (row, dict))
        {
            foreach (DataColumn col in table.Columns)
            {
                dict[col.ColumnName] = (Convert.ToString(row[col]));
            }

            data.Add(dict);
        }

        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }
}