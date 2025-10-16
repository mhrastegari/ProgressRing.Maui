public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseProgressRing(this MauiAppBuilder builder)
	{
		builder.ConfigureMauiHandlers(handlers =>
		{
#if ANDROID
    		handlers.AddHandler<Maui.ProgressRing.ProgressRing, Maui.ProgressRing.Platform.Android.ProgressRingHandler>();
#endif

#if IOS || MACCATALYST
			handlers.AddHandler<Maui.ProgressRing.ProgressRing, Maui.ProgressRing.Platform.MaciOS.ProgressRingHandler>();
#endif

#if WINDOWS
    		handlers.AddHandler<Maui.ProgressRing.ProgressRing, Maui.ProgressRing.Platform.Windows.ProgressRingHandler>();
#endif
		});

		return builder;
	}
}
