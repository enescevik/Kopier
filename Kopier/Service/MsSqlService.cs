using System.Data;
using System.Data.SqlClient;

namespace Kopier.Service;

internal sealed class MsSqlService : ServiceBase
{
    internal MsSqlService(string connectionString) => DataAdapter = new SqlDataAdapter(null, connectionString);

    public override bool TransferData(List<DataRow> rows, string targetTableName) => throw new NotImplementedException();
}