using Android.Views;
using Android.Widget;
using Google.Android.Material.ProgressIndicator;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Maui.ProgressRing.Platform.Android;

public partial class ProgressRingHandler : ViewHandler<ProgressRing, FrameLayout>
{
    const double DefaultSize = 48.0;
    const double DefaultStroke = 4.0;

    CircularProgressIndicator? _indicator;

    public ProgressRingHandler() : base(Mapper, CommandMapper) { }

    public static IPropertyMapper<ProgressRing, ProgressRingHandler> Mapper =
        new PropertyMapper<ProgressRing, ProgressRingHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ProgressRing.IsIndeterminate)] = MapIsIndeterminate,
            [nameof(ProgressRing.Progress)] = MapProgress,
            [nameof(ProgressRing.ProgressColor)] = MapProgressColor,
            [nameof(ProgressRing.TrackColor)] = MapTrackColor,
            [nameof(ProgressRing.StrokeThickness)] = MapStrokeThickness
        };

    public static CommandMapper<ProgressRing, ProgressRingHandler> CommandMapper =
        new(ViewHandler.ViewCommandMapper) { };

    protected override FrameLayout CreatePlatformView()
    {
        var container = new FrameLayout(Context);

        _indicator = new CircularProgressIndicator(Context)
        {
            Max = 1000,
            Indeterminate = false
        };

        var size = (int)DpToPixels(DefaultSize);
        var layoutParams = new FrameLayout.LayoutParams(size, size)
        {
            Gravity = GravityFlags.Center
        };

        _indicator.LayoutParameters = layoutParams;
        container.AddView(_indicator);

        return container;
    }

    protected override void ConnectHandler(FrameLayout platformView)
    {
        base.ConnectHandler(platformView);
        UpdateAll();
    }

    void UpdateAll()
    {
        if (VirtualView is null || _indicator is null) return;

        var size = VirtualView.WidthRequest > 0 ? VirtualView.WidthRequest : DefaultSize;
        var stroke = VirtualView.StrokeThickness > 0 ? VirtualView.StrokeThickness : DefaultStroke;

        _indicator.IndicatorSize = (int)DpToPixels(size);
        _indicator.TrackThickness = (int)DpToPixels(stroke);

        var progressColor = VirtualView.ProgressColor.ToPlatform();
        var trackColor = VirtualView.TrackColor.ToPlatform();

#if ANDROID33_0_OR_GREATER
        _indicator.SetIndicatorColor(progressColor);
        _indicator.TrackColor = trackColor;
#else
        _indicator.IndicatorColors = new int[] { progressColor };
        _indicator.TrackColor = trackColor;
#endif

        _indicator.Indeterminate = VirtualView.IsIndeterminate;

        if (!VirtualView.IsIndeterminate)
            _indicator.SetProgress((int)(VirtualView.Progress * _indicator.Max), false);
        else
            _indicator.SetProgress((int)(0.3 * _indicator.Max), false);
    }

    void UpdateProgress()
    {
        if (VirtualView is null || _indicator is null)
            return;

        if (!VirtualView.IsIndeterminate)
            _indicator.SetProgress((int)(VirtualView.Progress * _indicator.Max), true);
    }

    static float DpToPixels(double dp)
    {
        var metrics = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Resources?.DisplayMetrics;
        return metrics == null ? (float)dp : (float)(dp * metrics.Density);
    }

    public static void MapIsIndeterminate(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();
    public static void MapProgress(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateProgress();
    public static void MapProgressColor(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();
    public static void MapTrackColor(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();
    public static void MapStrokeThickness(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();
}
