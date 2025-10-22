namespace ProgressRing.Maui.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void ToggleIndeterminate_Clicked(object sender, EventArgs e)
	{
		DeterminateRing.IsIndeterminate = !DeterminateRing.IsIndeterminate;
	}
}
