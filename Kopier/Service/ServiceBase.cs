using System.Data;

namespace Kopier.Service;

public abstract class ServiceBase
{
    private IDbDataAdapter _dataAdapter;

    public abstract bool TransferData(List<DataRow> rows, string targetTableName);

    public Guid Id { get; private set; } = new Guid();
    public int ParallelCPU { get; set; }

    public IDbDataAdapter DataAdapter
    {
        get => _dataAdapter;
        internal set
        {
            value.SelectCommand.CommandTimeout = 0;
            _dataAdapter = value;
        }
    }

    private void SetConnectionState(bool open = true)
    {
        if (open && DataAdapter.SelectCommand.Connection.State != ConnectionState.Open)
        {
            DataAdapter.SelectCommand.Connection.Open();

            if (ParallelCPU > 0)
            {
                DataAdapter.SelectCommand.CommandText = $"ALTER SESSION FORCE PARALLEL QUERY PARALLEL {ParallelCPU}";
                DataAdapter.SelectCommand.ExecuteNonQuery();
            }
        }
        else if (!open && DataAdapter.SelectCommand.Connection.State != ConnectionState.Closed)
        {
            DataAdapter.SelectCommand.Connection.Close();
        }
    }

    public bool HasConnection()
    {
        try
        {
            SetConnectionState(false);

            DataAdapter.SelectCommand.Connection.Open();
            DataAdapter.SelectCommand.Connection.Close();

            return true;
        }
        catch
        {
            throw;
        }
    }

    public bool ExecuteNonQuery(string commandText)
    {
        try
        {
            SetConnectionState();
            DataAdapter.SelectCommand.CommandText = commandText;

            DataAdapter.SelectCommand.ExecuteNonQuery();
            return true;
        }
        catch
        {
            throw;
        }
        finally
        {
            SetConnectionState(false);
        }
    }

    public object ExecuteScalar(string commandText)
    {
        try
        {
            SetConnectionState();
            DataAdapter.SelectCommand.CommandText = commandText;

            return DataAdapter.SelectCommand.ExecuteScalar();
        }
        catch
        {
            throw;
        }
        finally
        {
            SetConnectionState(false);
        }
    }

    public DataTable FillDataTable(string commandText)
    {
        try
        {
            var ds = new DataSet();
            DataAdapter.SelectCommand.CommandText = commandText;
            DataAdapter.Fill(ds);

            return ds.Tables.Count <= 0 ? new DataTable() : ds.Tables[0];
        }
        catch
        {
            throw;
        }
    }

    public override string ToString() => DataAdapter.SelectCommand.Connection.ConnectionString;
}