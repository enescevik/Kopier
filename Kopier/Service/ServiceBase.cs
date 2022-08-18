using System.Data;

namespace Kopier.Service;

public abstract class ServiceBase
{
    private IDbDataAdapter _dataAdapter;

    public abstract Task<int> GetCountAsync(string query);
    public abstract Task<DataTable> GetDataAsync(string query, string keyFields = "", int skip = 0, int take = 0);
    public abstract Task<bool> TransferDataAsync(List<DataRow> rows, string targetTableName);

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

    private async Task SetConnectionStateAsync(bool open = true)
    {
        if (open && DataAdapter.SelectCommand.Connection.State != ConnectionState.Open)
        {
            await Task.Run(() => DataAdapter.SelectCommand.Connection.Open());

            if (ParallelCPU > 0)
            {
                DataAdapter.SelectCommand.CommandText = $"ALTER SESSION FORCE PARALLEL QUERY PARALLEL {ParallelCPU}";
                await Task.Run(() => DataAdapter.SelectCommand.ExecuteNonQuery());
            }
        }
        else if (!open && DataAdapter.SelectCommand.Connection.State != ConnectionState.Closed)
        {
            await Task.Run(() => DataAdapter.SelectCommand.Connection.Close());
        }
    }

    public async Task<bool> HasConnectionAsync()
    {
        try
        {
            await SetConnectionStateAsync(false);

            await Task.Run(() => DataAdapter.SelectCommand.Connection.Open());
            await Task.Run(() => DataAdapter.SelectCommand.Connection.Close());

            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExecuteNonQueryAsync(string query)
    {
        try
        {
            await SetConnectionStateAsync();
            DataAdapter.SelectCommand.CommandText = query;

            await Task.Run(() => DataAdapter.SelectCommand.ExecuteNonQuery());
            return true;
        }
        catch
        {
            throw;
        }
        finally
        {
            await SetConnectionStateAsync(false);
        }
    }

    public async Task<object> ExecuteScalarAsync(string query)
    {
        try
        {
            await SetConnectionStateAsync();
            DataAdapter.SelectCommand.CommandText = query;

            return await Task.Run(() => DataAdapter.SelectCommand.ExecuteScalar());
        }
        catch
        {
            throw;
        }
        finally
        {
            await SetConnectionStateAsync(false);
        }
    }

    public async Task<DataTable> FillDataTableAsync(string query)
    {
        try
        {
            var ds = new DataSet();
            DataAdapter.SelectCommand.CommandText = query;
            await Task.Run(() => DataAdapter.Fill(ds));

            return ds.Tables.Count <= 0 ? new DataTable() : ds.Tables[0];
        }
        catch
        {
            throw;
        }
    }

    public override string ToString() => DataAdapter.SelectCommand.Connection.ConnectionString;
}