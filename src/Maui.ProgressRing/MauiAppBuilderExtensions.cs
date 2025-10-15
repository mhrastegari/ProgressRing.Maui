namespace Maui.ProgressRing;

public static class MauiAppBuilderExtensions
{
	public static MauiAppBuilder UseProgressRing(this MauiAppBuilder builder)
	{
		builder.ConfigureMauiHandlers(handlers =>
		{
#if ANDROID
			handlers.AddHandler<ProgressRing, Platforms.Android.ProgressRingHandler>();
#elif IOS || MACCATALYST
            handlers.AddHandler<ProgressRing, Platforms.MaciOS.ProgressRingHandler>();
#elif WINDOWS
            handlers.AddHandler<ProgressRing, Platforms.Windows.ProgressRingHandler>();
#endif
		});

		return builder;
	}
}
