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
        var newName = await DisplayPromptAsync(
            "Rename List",
            "Enter a new name for this list",
            "Save",
            "Cancel",
            placeholder: _viewModel.Title,
            initialValue: _viewModel.Title,
            maxLength: 60);

        if (string.IsNullOrWhiteSpace(newName)) return;
        await _viewModel.RenameListAsync(newName.Trim());
    }
}
