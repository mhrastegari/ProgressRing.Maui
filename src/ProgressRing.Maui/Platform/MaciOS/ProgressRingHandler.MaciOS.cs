using CoreAnimation;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace ProgressRing.Maui.Platform.MaciOS;

public partial class ProgressRingHandler : ViewHandler<ProgressRing, UIView>
{
    private const double DefaultSize = 40;
    private const double DefaultStroke = 4;

    private CAShapeLayer? _trackLayer;
    private CAShapeLayer? _progressLayer;

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

    protected override UIView CreatePlatformView()
    {
        _trackLayer = CreateLayer(UIColor.QuaternaryLabel.CGColor, DefaultStroke, 1.0);
        _progressLayer = CreateLayer(UIColor.SystemBlue.CGColor, DefaultStroke, 0.0);

        var container = new UIView
        {
            Frame = new CGRect(0, 0,
                VirtualView?.WidthRequest > 0 ? VirtualView.WidthRequest : DefaultSize,
                VirtualView?.HeightRequest > 0 ? VirtualView.HeightRequest : DefaultSize),
        };

        container.Layer.AddSublayer(_trackLayer);
        container.Layer.AddSublayer(_progressLayer);

        return container;
    }

    protected override void ConnectHandler(UIView platformView)
    {
        base.ConnectHandler(platformView);

        if (VirtualView is not null)
        {
            VirtualView.SizeChanged += VirtualView_SizeChanged;
        }
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        base.DisconnectHandler(platformView);

        if (VirtualView is not null)
        {
            VirtualView.SizeChanged -= VirtualView_SizeChanged;
        }

        _progressLayer?.RemoveAnimation("rotationAnimation");
    }

    private void VirtualView_SizeChanged(object? sender, EventArgs e) => SetSizeAndLayers();

    private CAShapeLayer CreateLayer(CGColor color, double stroke, double strokeEnd)
    {
        return new CAShapeLayer
        {
            FillColor = UIColor.Clear.CGColor,
            StrokeColor = color,
            LineWidth = (float)stroke,
            StrokeEnd = (float)strokeEnd,
            LineCap = CAShapeLayer.CapRound,
            AnchorPoint = new CGPoint(0.5, 0.5)
        };
    }

    private void SetSizeAndLayers()
    {
        if (VirtualView is null || PlatformView is null) return;

        var width = PlatformView.Bounds.Width;
        var height = PlatformView.Bounds.Height;
        var center = new CGPoint(width / 2.0, height / 2.0);
        var stroke = (float)(VirtualView.StrokeThickness > 0 ? VirtualView.StrokeThickness : DefaultStroke);
        var radius = (float)(Math.Max(0, Math.Min(width, height) / 2.0 - stroke / 2.0));
        var startAngle = (float)(-Math.PI / 2.0);
        var endAngle = (float)(3.0 * Math.PI / 2.0);
        var circlePath = UIBezierPath.FromArc(center, radius, startAngle, endAngle, true).CGPath;

        if (_trackLayer is null || _progressLayer is null) return;

        _trackLayer.Frame = PlatformView.Bounds;
        _trackLayer.Path = circlePath;
        _trackLayer.LineWidth = stroke;
        _trackLayer.StrokeColor = VirtualView.TrackColor?.ToPlatform()?.CGColor ?? UIColor.QuaternaryLabel.CGColor;

        _progressLayer.Frame = PlatformView.Bounds;
        _progressLayer.Path = circlePath;
        _progressLayer.LineWidth = stroke;
        _progressLayer.StrokeColor = VirtualView.ProgressColor?.ToPlatform()?.CGColor ?? UIColor.SystemBlue.CGColor;

        _progressLayer.RemoveAnimation("rotationAnimation");

        if (VirtualView.IsIndeterminate)
        {
            StartIndeterminateAnimation();
        }

        SetProgress();
    }

    private void SetProgress()
    {
        if (_progressLayer is null) return;

        var progress = VirtualView.IsIndeterminate ? 0.75 : Math.Max(0.0, Math.Min(1.0, VirtualView.Progress));

        var animation = CABasicAnimation.FromKeyPath("strokeEnd");
        animation.From = NSNumber.FromDouble(_progressLayer.StrokeEnd);
        animation.To = NSNumber.FromDouble(progress);
        animation.Duration = 0.25;
        animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);

        _progressLayer.AddAnimation(animation, "strokeEnd");
        _progressLayer.StrokeEnd = (float)progress;
    }

    private void StartIndeterminateAnimation()
    {
        if (_progressLayer?.AnimationForKey("rotationAnimation") is not null) return;

        var rotation = CABasicAnimation.FromKeyPath("transform.rotation.z");
        rotation.From = NSNumber.FromDouble(0);
        rotation.To = NSNumber.FromDouble(2.0 * Math.PI);
        rotation.Duration = 1.0;
        rotation.RepeatCount = float.MaxValue;
        rotation.RemovedOnCompletion = false;

        _progressLayer?.AddAnimation(rotation, "rotationAnimation");
    }

    public static void MapIsIndeterminate(ProgressRingHandler handler, ProgressRing view) => handler?.SetSizeAndLayers();
    public static void MapStrokeThickness(ProgressRingHandler handler, ProgressRing view) => handler?.SetSizeAndLayers();
    public static void MapProgressColor(ProgressRingHandler handler, ProgressRing view) => handler?.SetSizeAndLayers();
    public static void MapTrackColor(ProgressRingHandler handler, ProgressRing view) => handler?.SetSizeAndLayers();
    public static void MapSize(ProgressRingHandler handler, ProgressRing view) => handler?.SetSizeAndLayers();
    public static void MapProgress(ProgressRingHandler handler, ProgressRing view) => handler?.SetProgress();
}
