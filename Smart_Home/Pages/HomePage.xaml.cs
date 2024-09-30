using Smart_Home.ViewModels;

namespace Smart_Home.Pages;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}