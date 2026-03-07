using CartMule.ViewModels;

namespace CartMule.Views;

public partial class ListDetailPage : ContentPage
{
    private readonly ListDetailViewModel _viewModel;

    public ListDetailPage(ListDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadItemsCommand.Execute(null);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"additem?listId={_viewModel.ListId}");
    }

    private async void OnRenameClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"addlist?listId={_viewModel.ListId}");
    }
}
