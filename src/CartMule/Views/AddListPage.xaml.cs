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

    private async void OnEditCategoryClicked(object sender, EventArgs e)
    {
        if (sender is not BindableObject b || b.BindingContext is not CategoryEditItem cat) return;

        var newName = await DisplayPromptAsync(
            "Rename Category",
            "Enter a new name",
            "Save",
            "Cancel",
            initialValue: cat.Name,
            maxLength: 60);

        if (string.IsNullOrWhiteSpace(newName)) return;
        await _viewModel.RenameCategoryAsync(cat, newName.Trim());
    }
}
