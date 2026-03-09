using CartMule.ViewModels;

namespace CartMule.Views;

public partial class AddListPage : ContentPage
{
    private readonly AddListViewModel _viewModel;

    public AddListPage(AddListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitialiseCommand.Execute(null);
    }

    private async void OnCategoryUnfocused(object sender, FocusEventArgs e)
    {
        if (sender is not BindableObject b || b.BindingContext is not CategoryEditItem cat) return;
        await _viewModel.PersistCategoryOnFocusLostAsync(cat);
    }

    private async void OnShopUnfocused(object sender, FocusEventArgs e)
    {
        if (sender is not BindableObject b || b.BindingContext is not ShopEditItem shop) return;
        await _viewModel.PersistShopOnFocusLostAsync(shop);
    }
}
