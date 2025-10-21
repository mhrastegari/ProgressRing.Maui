using System.Runtime.InteropServices;
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
            ClipsToBounds = true,
            Frame = new CGRect(0, 0,
                VirtualView?.WidthRequest > 0 ? VirtualView.WidthRequest : DefaultSize,
                VirtualView?.HeightRequest > 0 ? VirtualView.HeightRequest : DefaultSize),
            BackgroundColor = VirtualView?.BackgroundColor?.ToPlatform() ?? UIColor.Clear,
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

        platformView.InvokeOnMainThread(() => SetSizeAndLayers());
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

        var width = VirtualView.WidthRequest > 0 ? VirtualView.WidthRequest : PlatformView.Bounds.Width;
        var height = VirtualView.HeightRequest > 0 ? VirtualView.HeightRequest : PlatformView.Bounds.Height;

        if ((width <= 0 || height <= 0) && PlatformView.Superview is not null)
        {
            width = PlatformView.Superview.Bounds.Width;
            height = PlatformView.Superview.Bounds.Height;
        }

        if (width <= 0 || height <= 0)
        {
            width = height = DefaultSize;
        }

        var size = Math.Max(width, height);

        PlatformView.Bounds = new CGRect(0, 0, width, height);
        PlatformView.Frame = new CGRect(0, 0, width, height);

        var stroke = VirtualView.StrokeThickness > 0 ? VirtualView.StrokeThickness : DefaultStroke;
        var center = new CGPoint(width / 2.0, height / 2.0);
        var radius = Math.Max(0, Math.Min(width, height) / 2.0 - stroke / 2.0);
        var startAngle = -Math.PI / 2.0;
        var endAngle = 3.0 * Math.PI / 2.0;
        var circlePath = UIBezierPath.FromArc(center, (NFloat)radius, (NFloat)startAngle, (NFloat)endAngle, true);

        if (_trackLayer is null || _progressLayer is null) return;

        _trackLayer.Frame = PlatformView.Bounds;
        _trackLayer.Path = circlePath.CGPath;
        _trackLayer.LineWidth = (float)stroke;
        _trackLayer.StrokeColor = VirtualView.TrackColor?.ToPlatform()?.CGColor ?? UIColor.QuaternaryLabel.CGColor;

        _progressLayer.Frame = PlatformView.Bounds;
        _progressLayer.Path = circlePath.CGPath;
        _progressLayer.LineWidth = (float)stroke;
        _progressLayer.StrokeColor = VirtualView.ProgressColor?.ToPlatform()?.CGColor ?? UIColor.SystemBlue.CGColor;

        _progressLayer.RemoveAnimation("rotationAnimation");

        if (VirtualView.IsIndeterminate)
        {
            SetProgress(0.25);
            StartIndeterminateAnimation();
        }
        else
        {
            SetProgress(VirtualView.Progress);
        }

        PlatformView.SetNeedsDisplay();
    }

    private void SetProgress(double progress, bool animate = true)
    {
        if (_progressLayer is null) return;

        progress = Math.Max(0.0, Math.Min(1.0, progress));

        if (animate)
        {
            var animation = CABasicAnimation.FromKeyPath("strokeEnd");
            animation.From = NSNumber.FromFloat((float)_progressLayer.StrokeEnd);
            animation.To = NSNumber.FromFloat((float)progress);
            animation.Duration = 0.25;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
            _progressLayer.AddAnimation(animation, "strokeEnd");
        }

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
    public static void MapProgress(ProgressRingHandler handler, ProgressRing view) => handler?.SetProgress(view.Progress);
}
