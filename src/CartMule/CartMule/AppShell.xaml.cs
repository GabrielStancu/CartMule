using CartMule.Views;

namespace CartMule;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("listdetail", typeof(ListDetailPage));
    }
}
