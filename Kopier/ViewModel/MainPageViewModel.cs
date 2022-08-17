using Kopier.Model;
using Kopier.Service;
using MySqlX.XDevAPI.Common;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Kopier.ViewModel;

public class MainPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public List<ConnectionModel> SourceConnections { get; set; } = new()
    {
        new("bevdbtest03", "Data Source=bevdbtest03;Initial Catalog=CapuaDB_Test;User ID=testsql;Password=test2!IT;MultipleActiveResultSets=True;Connection Timeout=15;Max Pool Size=200;"),
        new("arktestdb", "Server=arktestdb.cs.local;Database=TercumanDB;Uid=sqlsa;Pwd=Super61!;SslMode=none;")
    };

    public List<ConnectionModel> TargetConnections { get; set; } = new()
    {
        new("arktest-mizu01", "Server=arktest-mizu01;Uid=user_test;Pwd=YtWuSy5TxQr5fLrAXuP92z2XHuCX9e;SslMode=none;")
    };

    public List<string> TargetDatabases { get; set; } = new()
    {
        "MizuProductDB", "MizuExtraProductDB", "MizuCategoryDB", "MizuBranchDB"
    };

    public List<QueryGroupModel> Queries { get; set; }

    public ICommand OnLoadCommand { get; private set; }
    public ICommand OnSaveCommand { get; private set; }
    public ICommand OnAddCommand { get; private set; }
    public ICommand OnRemoveCommand { get; private set; }
    public ICommand OnTransferCommand { get; private set; }
    public ICommand OnQueryCommand { get; private set; }

    public MainPageViewModel()
    {
        Queries = new()
        {
            new("ExtraProduct Sorguları", new()
            {
                new() { Name = "ExtraProduct", Source = SourceConnections[0], Target = TargetConnections[0], Database = TargetDatabases[2],
                    TableName = "ExtraProduct", Query = @"Select
    EP.PKExtraProductId Id,
    EP.ProductCode Code,
    EP.Name,
    EP.Sequence,
    EP.OSDeliverType DeliveryType,
    EP.IsActiveSpecial,
    EP.IsActive,
    EP.FKCreatedBy CreatedBy,
    EP.FKModifiedBy ModifiedBy,
    EP.CreatedOn,
    EP.ModifiedOn
From STExtraProduct EP
Inner Join STExtraProductWebSite WS On WS.FKExtraProductId = EP.PKExtraProductId And WS.IsActive = 1 And WS.FKWebSiteId = 11
Where EP.IsActive = 1 And EP.Name Not Like '%test%' And EP.Name != 'tesetetste'" },
                new() { Name = "ExtraProductPrice", Source = SourceConnections[0], Target = TargetConnections[0], Database = TargetDatabases[1],
                    TableName = "ExtraProductPrice", Query = @"Select
    FKExtraProductId ExtraProductId,
    FKCountryId CountryId,
    BuyingPrice,
    TotalPrice,
    Kdv,
    IEPSRate,
    Cost,
    IsActive,
    FKCreatedBy CreatedBy,
    FKModifiedBy ModifiedBy,
    CreatedOn,
    ModifiedOn
From STExtraProductPrice
Where FKWebSiteId = 11 And IsActive = 1 And FKCountryId In (18, 32, 51, 76)" },
                new() { Name = "ExtraProductImage", Source = SourceConnections[0], Target = TargetConnections[0], Database = TargetDatabases[1],
                    TableName = "ExtraProductImage", Query = @"Select
    FKExtraProductId ExtraProductId,
    Image Path,
    IsActive,
    FKCreatedBy CreatedBy,
    FKModifiedBy ModifiedBy,
    CreatedOn,
    ModifiedOn
From STExtraProductImage
Where FKWebSiteId = 11 And IsActive = 1" },
                new() { Name = "ExtraProductBranch", Source = SourceConnections[0], Target = TargetConnections[0], Database = TargetDatabases[1],
                    TableName = "ExtraProductBranch", Query = @"Select
    FKExtraProductId ExtraProductId,
    FKBranchId BranchId,
    StockQuantity,
    IsUseStockQuantity,
    IsActive,
    FKCreatedBy CreatedBy,
    FKModifiedBy ModifiedBy,
    CreatedOn,
    ModifiedOn
FROM STExtraProductBranch
Where FKWebSiteId = 11 And IsActive = 1" },
                new() { Name = "TercumanDBContent", Source = SourceConnections[1], Target = TargetConnections[0], Database = TargetDatabases[1],
                    TableName = "", Query = @"Select
    Cast(Identifier As Signed Integer) ExtraProductId,
    Case LanguageCode
        When 'ENG' Then 'EN'
        When 'SPA' Then 'ES'
        When 'MEX' Then 'MX'
        When 'GER' Then 'DE'
        When 'COL' Then 'CO'
        Else LanguageCode
    End LanguageCode,
    FieldName,
    ContentValue Value,
    1 IsActive,
    1 CreatedBy,
    1 ModifiedBy,
    Now() CreatedOn
From TercumanDB.Content
Where LanguageCode In ('ENG', 'SPA', 'MEX', 'COL', 'GER')
    And ContainerName ='ExtraProduct'
    And FieldName = 'Name'
    And ApplicationName ='10405310-9821-4915-bc97-4671a8442af6'" }
            }),
            new("Branch", new()
            {
                new() { Name = "BranchRegionRelation", Source = SourceConnections[0], Target = TargetConnections[0], Database = TargetDatabases[3],
                    TableName = "BranchRegionRelation", Query = @"Select
    PKBranchRegionRelationId Id,
    BR.FKBranchId BranchId,
    FKRegionId RegionId,
    DeliveryCharge,
    CloseTime,
    OSDistanceType,
    DeliveryChargeAllowance,
    BR.IsActive,
    BR.FKCreatedBy CreatedBy,
    BR.FKModifiedBy ModifiedBy,
    BR.CreatedOn,
    BR.ModifiedOn,
    PC.FKProductCategoryId CategoryId
From STBranchRegionRelation BR
Inner Join STBranchWebSite BWS On BWS.FKBranchId = BR.FKBranchId And BWS.IsActive = 1 And BWS.FKWebSiteId = 11
Left Join STProductGroup PG On PG.PKProductGroupId = BR.FKProductGroupId
Left Join MTProductCategory PC On PC.Name = PG.Name
Where BR.IsActive = 1 And BR.FKProductGroupId = 1" }
            }),
            new("Product", new()
            {
                new() { Name = "ProductCategory", Source = SourceConnections[0], Target = TargetConnections[0], Database = TargetDatabases[0],
                    TableName = "ProductCategory", Query = @"Select
    PG.PKProductGroupId,
    PG.Name,
    PC.PKProductCategoryId,
    PC.Name
From STProductGroup PG
Inner Join MTProductCategory PC on PC.Name = PG.Name" }
            })
        };

        OnLoadCommand = new Command(Execute_OnLoadCommand);
        OnSaveCommand = new Command(Execute_OnSaveCommand);
        OnAddCommand = new Command(Execute_OnAddCommand);
        OnRemoveCommand = new Command(Execute_OnRemoveCommand);
        OnTransferCommand = new Command(Execute_OnTransferCommand);
        OnQueryCommand = new Command(Execute_OnQueryCommand);
    }

    private ConnectionModel _sourceConnection;
    public ConnectionModel SourceConnection
    {
        get => _sourceConnection;
        set
        {
            if (_sourceConnection != value)
            {
                _sourceConnection = value;
                OnPropertyChanged();
            }
        }
    }

    private ConnectionModel _targetConnection;
    public ConnectionModel TargetConnection
    {
        get => _targetConnection;
        set
        {
            if (_targetConnection != value)
            {
                _targetConnection = value;
                OnPropertyChanged();
            }
        }
    }

    private string _targetDatabase;
    public string TargetDatabase
    {
        get => _targetDatabase;
        set
        {
            if (_targetDatabase != value)
            {
                _targetDatabase = value;
                OnPropertyChanged();
            }
        }
    }

    private string _targetTableName;
    public string TargetTableName
    {
        get => _targetTableName;
        set
        {
            if (_targetTableName != value)
            {
                _targetTableName = value;
                OnPropertyChanged();
            }
        }
    }

    private string _resultQuery;
    public string ResultQuery
    {
        get => _resultQuery;
        set
        {
            if (_resultQuery != value)
            {
                _resultQuery = value;
                OnPropertyChanged();
            }
        }
    }

    private string _editorQuery;
    public string EditorQuery
    {
        get => _editorQuery;
        set
        {
            if (_editorQuery != value)
            {
                _editorQuery = value;
                OnPropertyChanged();
            }
        }
    }

    private QueryModel _selectedQuery;
    public QueryModel SelectedQuery
    {
        get => _selectedQuery;
        set
        {
            if (_selectedQuery != value)
            {
                _selectedQuery = value;

                EditorQuery = _selectedQuery?.Query;
                SourceConnection = _selectedQuery?.Source;
                TargetConnection = _selectedQuery?.Target;
                TargetDatabase = _selectedQuery?.Database;
                TargetTableName = _selectedQuery?.TableName;

                OnPropertyChanged();
            }
        }
    }

    private bool _working;
    public bool Working
    {
        get => _working;
        set
        {
            if (_working != value)
            {
                _working = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _notWorking = true;
    public bool NotWorking
    {
        get => _notWorking;
        set
        {
            if (_notWorking != value)
            {
                _notWorking = value;
                OnPropertyChanged();
            }
        }
    }

    public void Execute_OnLoadCommand() { }
    public void Execute_OnSaveCommand() { }
    public void Execute_OnAddCommand() { }
    public void Execute_OnRemoveCommand() { }
    public void Execute_OnTransferCommand()
    {
        if (SourceConnection is not null && TargetConnection is not null)
        {
            try
            {
                Working = true;
                NotWorking = false;

                ServiceBase sourceService = SourceConnection.Name == "bevdbtest03"
                    ? new MsSqlService(SourceConnection.ConnectionString)
                    : new MySqlService(SourceConnection.ConnectionString);

                var data = sourceService.FillDataTable(EditorQuery);

                var targetService = new MySqlService($"{TargetConnection.ConnectionString}Database={TargetDatabase};");
                targetService.TransferData(data.AsEnumerable().ToList(), TargetTableName);

                App.AlertService.ShowAlert("Transfer", "Transfer işlemi tamamlandı.");
            }
            catch (Exception ex)
            {
                App.AlertService.ShowAlert("Transfer Hata", $"{ex.Message}");
            }
            finally
            {
                Working = false;
                NotWorking = true;
            }
        }
    }

    public void Execute_OnQueryCommand()
    {
        if (SourceConnection is not null && !string.IsNullOrWhiteSpace(EditorQuery))
        {
            ServiceBase sourceService = SourceConnection.Name == "bevdbtest03"
                ? new MsSqlService(SourceConnection.ConnectionString)
                : new MySqlService(SourceConnection.ConnectionString);

            ResultQuery = sourceService.FillDataTable(EditorQuery).ToJson();
        }
    }

    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}