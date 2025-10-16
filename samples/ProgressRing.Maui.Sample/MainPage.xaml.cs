namespace ProgressRing.Maui.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void ProgressSlider_ValueChanged(object sender, ValueChangedEventArgs e)
	{
		if (DeterminateRing is null) return;
		DeterminateRing.Progress = e.NewValue;
	}
}
