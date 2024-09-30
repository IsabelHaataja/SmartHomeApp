using SmartHomeForIot.ViewModels;

namespace SmartHomeForIot.Pages;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}