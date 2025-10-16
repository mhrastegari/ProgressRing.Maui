public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseProgressRing(this MauiAppBuilder builder)
	{
		builder.ConfigureMauiHandlers(handlers =>
		{
#if ANDROID
    		handlers.AddHandler<ProgressRing.Maui.ProgressRing, ProgressRing.Maui.Platform.Android.ProgressRingHandler>();
#endif

#if IOS || MACCATALYST
			handlers.AddHandler<ProgressRing.Maui.ProgressRing, ProgressRing.Maui.Platform.MaciOS.ProgressRingHandler>();
#endif

#if WINDOWS
    		handlers.AddHandler<ProgressRing.Maui.ProgressRing, ProgressRing.Maui.Platform.Windows.ProgressRingHandler>();
#endif
		});

		return builder;
	}
}
