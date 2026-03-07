using CartMule.ViewModels;

namespace CartMule.Views;

public partial class ListsDashboardPage : ContentPage
{
    private readonly ListsDashboardViewModel _viewModel;

    public ListsDashboardPage(ListsDashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadListsCommand.Execute(null);
    }

    private async void OnAddListClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("addlist?listId=0");
    }
}
