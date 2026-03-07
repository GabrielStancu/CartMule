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
        var name = await DisplayPromptAsync(
            "New List",
            "Enter a name for your list",
            "Create",
            "Cancel",
            placeholder: "e.g. Weekly Groceries",
            maxLength: 60);

        if (string.IsNullOrWhiteSpace(name)) return;
        await _viewModel.CreateListAsync(name.Trim());
    }
}
