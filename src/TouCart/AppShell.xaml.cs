using TouCart.Views;

namespace TouCart;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("listdetail", typeof(ListDetailPage));
        Routing.RegisterRoute("additem", typeof(AddItemPage));
        Routing.RegisterRoute("addlist", typeof(AddListPage));
    }
}
