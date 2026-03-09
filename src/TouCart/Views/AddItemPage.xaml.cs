using TouCart.ViewModels;

namespace TouCart.Views;

public partial class AddItemPage : ContentPage
{
    private readonly AddItemViewModel _viewModel;

    public AddItemPage(AddItemViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _viewModel.InitialiseCommand.Execute(null);
    }
}
