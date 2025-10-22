using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using WinUIProgressRing = Microsoft.UI.Xaml.Controls.ProgressRing;
using WinUIViewbox = Microsoft.UI.Xaml.Controls.Viewbox;

namespace ProgressRing.Maui.Platform.Windows;

public partial class ProgressRingHandler : ViewHandler<ProgressRing, WinUIViewbox>
{
    private WinUIProgressRing? _progressRing;

    public ProgressRingHandler() : base(Mapper, CommandMapper) { }

    public static IPropertyMapper<ProgressRing, ProgressRingHandler> Mapper =
        new PropertyMapper<ProgressRing, ProgressRingHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ProgressRing.IsIndeterminate)] = MapIsIndeterminate,
            [nameof(ProgressRing.ProgressColor)] = MapProgressColor,
            [nameof(ProgressRing.TrackColor)] = MapTrackColor,
            [nameof(ProgressRing.HeightRequest)] = MapSize,
            [nameof(ProgressRing.WidthRequest)] = MapSize,
            [nameof(ProgressRing.Progress)] = MapProgress,
        };

    public static CommandMapper<ProgressRing, ProgressRingHandler> CommandMapper = new(ViewHandler.ViewCommandMapper) { };

    protected override WinUIViewbox CreatePlatformView()
    {
        return new WinUIViewbox
        {
            Child = _progressRing = new WinUIProgressRing { IsActive = true },
            Stretch = Microsoft.UI.Xaml.Media.Stretch.None
        };
    }

    protected override void ConnectHandler(WinUIViewbox platformView)
    {
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(WinUIViewbox platformView)
    {
        base.DisconnectHandler(platformView);
        _progressRing = null;
    }

    public static void MapIsIndeterminate(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._progressRing is null || view is null) return;
        handler._progressRing.IsIndeterminate = view.IsIndeterminate;
    }

    public static void MapProgressColor(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._progressRing is null || view.ProgressColor is null) return;
        handler._progressRing.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(view.ProgressColor.ToWindowsColor());
    }

    public static void MapTrackColor(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._progressRing is null || view.TrackColor is null) return;
        handler._progressRing.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(view.TrackColor.ToWindowsColor());
    }

    public static void MapProgress(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._progressRing is null || view is null) return;
        handler._progressRing.Value = view.Progress * 100;
    }

    public static void MapSize(ProgressRingHandler handler, ProgressRing view)
    {
        if (handler?._progressRing is null || view is null) return;

        var width = view.WidthRequest;
        var height = view.HeightRequest;

        if (width > 0 || height > 0)
        {
            var size = Math.Max(width, height);
            handler._progressRing.Width = size;
            handler._progressRing.Height = size;
        }
        else
        {
            handler._progressRing.ClearValue(Microsoft.UI.Xaml.FrameworkElement.WidthProperty);
            handler._progressRing.ClearValue(Microsoft.UI.Xaml.FrameworkElement.HeightProperty);
        }
    }
}
