using Kopier.Service;

namespace Kopier;

public partial class App : Application
{
    public static IServiceProvider Services;
    public static IAlertService AlertService;

    public App(IServiceProvider provider)
    {
        InitializeComponent();

        Services = provider;
        AlertService = Services.GetService<IAlertService>();

        MainPage = new AppShell();
    }
}
