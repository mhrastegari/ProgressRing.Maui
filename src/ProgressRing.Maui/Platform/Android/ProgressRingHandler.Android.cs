using Android.Views;
using Android.Widget;
using Google.Android.Material.ProgressIndicator;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace ProgressRing.Maui.Platform.Android;

public partial class ProgressRingHandler : ViewHandler<ProgressRing, FrameLayout>
{
    private CircularProgressIndicator? _indicator;

    public ProgressRingHandler() : base(Mapper, CommandMapper) { }

    public static IPropertyMapper<ProgressRing, ProgressRingHandler> Mapper =
        new PropertyMapper<ProgressRing, ProgressRingHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ProgressRing.IsIndeterminate)] = MapIsIndeterminate,
            [nameof(ProgressRing.StrokeThickness)] = MapStrokeThickness,
            [nameof(ProgressRing.ProgressColor)] = MapProgressColor,
            [nameof(ProgressRing.TrackColor)] = MapTrackColor,
            [nameof(ProgressRing.HeightRequest)] = MapSize,
            [nameof(ProgressRing.WidthRequest)] = MapSize,
            [nameof(ProgressRing.Progress)] = MapProgress,
        };

    public static CommandMapper<ProgressRing, ProgressRingHandler> CommandMapper = new(ViewHandler.ViewCommandMapper) { };

    protected override FrameLayout CreatePlatformView()
    {
        _indicator = new CircularProgressIndicator(Context)
        {
            Max = 1000,
        };

        var container = new FrameLayout(Context);
        container.AddView(_indicator);
        return container;
    }

    protected override void ConnectHandler(FrameLayout platformView)
    {
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(FrameLayout platformView)
    {
        base.DisconnectHandler(platformView);
        _indicator = null;
    }

    private static float DpToPixels(double dp)
    {
        var metrics = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Resources?.DisplayMetrics;
        return metrics is null ? (float)dp : (float)(dp * metrics.Density);
    }

    public static void MapIsIndeterminate(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._indicator is null) return;
        handler._indicator.Indeterminate = view.IsIndeterminate;
    }

    public static void MapStrokeThickness(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._indicator is null || view.StrokeThickness <= 0) return;
        handler._indicator.TrackThickness = (int)DpToPixels(view.StrokeThickness);
    }

    public static void MapProgressColor(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._indicator is null || view.ProgressColor is null) return;
        handler._indicator.SetIndicatorColor(view.ProgressColor.ToPlatform());
    }

    public static void MapTrackColor(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._indicator is null || view.TrackColor is null) return;
        handler._indicator.TrackColor = view.TrackColor.ToPlatform();
    }

    public static void MapProgress(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._indicator is null || view is null) return;

        var progress = (int)(view.Progress * handler._indicator.Max);

        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            handler._indicator.SetProgress(progress, true);
        }
        else
        {
            handler._indicator.Progress = progress;
        }
    }

    public static void MapSize(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._indicator is null || view is null) return;

        var width = view.WidthRequest;
        var height = view.HeightRequest;

        if (width > 0 || height > 0)
        {
            var size = (int)DpToPixels(Math.Max(width, height));
            handler._indicator.IndicatorSize = size;
            handler._indicator.LayoutParameters = new FrameLayout.LayoutParams(size, size)
            {
                Gravity = GravityFlags.Center
            };
        }
    }
}
