using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using WinUIProgressRingView = Microsoft.UI.Xaml.Controls.ProgressRing;

namespace Maui.ProgressRing.Platforms.Windows;

public partial class ProgressRingHandler : ViewHandler<ProgressRing, WinUIProgressRingView>
{
    public ProgressRingHandler() : base(Mapper, CommandMapper) { }

    public static IPropertyMapper<ProgressRing, ProgressRingHandler> Mapper =
        new PropertyMapper<ProgressRing, ProgressRingHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ProgressRing.IsIndeterminate)] = MapIsIndeterminate,
            [nameof(ProgressRing.Progress)] = MapProgress,
            [nameof(ProgressRing.ProgressColor)] = MapProgressColor,
            [nameof(ProgressRing.TrackColor)] = MapTrackColor
        };

    public static CommandMapper<ProgressRing, ProgressRingHandler> CommandMapper = new(ViewHandler.ViewCommandMapper) { };

    protected override WinUIProgressRingView CreatePlatformView()
    {
        var pr = new WinUIProgressRingView();

        var width = VirtualView?.WidthRequest > 0 ? VirtualView.WidthRequest : 40;
        var height = VirtualView?.HeightRequest > 0 ? VirtualView.HeightRequest : 40;

        pr.Width = width;
        pr.Height = height;

        pr.IsActive = true;
        return pr;
    }

    protected override void ConnectHandler(WinUIProgressRingView platformView)
    {
        base.ConnectHandler(platformView);
        UpdateAll();
    }

    void UpdateAll()
    {
        if (VirtualView == null || PlatformView == null) return;

        var width = VirtualView.WidthRequest > 0 ? VirtualView.WidthRequest : 40;
        var height = VirtualView.HeightRequest > 0 ? VirtualView.HeightRequest : 40;

        PlatformView.Width = width;
        PlatformView.Height = height;

        PlatformView.IsActive = true;
        PlatformView.IsIndeterminate = VirtualView.IsIndeterminate;

        if (!VirtualView.IsIndeterminate)
            PlatformView.Value = VirtualView.Progress * 100;

        UpdateProgressColor();
        UpdateTrackColor();
    }

    void UpdateProgressColor()
    {
        if (VirtualView?.ProgressColor is null) return;

        var brush = new Microsoft.UI.Xaml.Media.SolidColorBrush(VirtualView.ProgressColor.ToWindowsColor());
        PlatformView.Foreground = brush;
    }

    void UpdateTrackColor()
    {
        if (VirtualView?.TrackColor is null) return;

        var brush = new Microsoft.UI.Xaml.Media.SolidColorBrush(VirtualView.TrackColor.ToWindowsColor());
        PlatformView.Background = brush;
    }

    public static void MapIsIndeterminate(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();

    public static void MapProgress(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();

    public static void MapProgressColor(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();

    public static void MapTrackColor(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateAll();
}
