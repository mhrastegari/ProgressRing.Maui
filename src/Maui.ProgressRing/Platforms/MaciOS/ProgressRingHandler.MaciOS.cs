using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;
using MauiCircularProgress.Controls;
using Foundation;
using System.Runtime.InteropServices;

namespace Maui.ProgressRing.Platforms.MaciOS
{
    public partial class ProgressRingHandler : ViewHandler<ProgressRing, UIView>
    {
        const double DefaultSize = 40;
        const double DefaultStroke = 4;

        private CAShapeLayer _trackLayer;
        private CAShapeLayer _progressLayer;

        public ProgressRingHandler() : base(Mapper, CommandMapper) { }

        public static IPropertyMapper<ProgressRing, ProgressRingHandler> Mapper =
            new PropertyMapper<ProgressRing, ProgressRingHandler>(ViewHandler.ViewMapper)
            {
                [nameof(ProgressRing.Progress)] = MapProgress,
                [nameof(ProgressRing.ProgressColor)] = MapProgressColor,
                [nameof(ProgressRing.TrackColor)] = MapTrackColor,
                [nameof(ProgressRing.StrokeThickness)] = MapStrokeThickness,
                [nameof(ProgressRing.IsIndeterminate)] = MapIsIndeterminate,
                [nameof(ProgressRing.WidthRequest)] = MapSize,
                [nameof(ProgressRing.HeightRequest)] = MapSize
            };

        public static CommandMapper<ProgressRing, ProgressRingHandler> CommandMapper =
            new(ViewHandler.ViewCommandMapper) { };

        protected override UIView CreatePlatformView()
        {
            var view = new UIView
            {
                Frame = new CGRect(0, 0,
                    VirtualView?.WidthRequest > 0 ? VirtualView.WidthRequest : DefaultSize,
                    VirtualView?.HeightRequest > 0 ? VirtualView.HeightRequest : DefaultSize)
            };

            _trackLayer = new CAShapeLayer
            {
                FillColor = UIColor.Clear.CGColor,
                StrokeColor = UIColor.LightGray.CGColor,
                LineCap = CAShapeLayer.CapRound,
                LineWidth = (float)DefaultStroke,
                AnchorPoint = new CGPoint(0.5, 0.5)
            };

            _progressLayer = new CAShapeLayer
            {
                FillColor = UIColor.Clear.CGColor,
                StrokeColor = UIColor.SystemPink.CGColor,
                LineCap = CAShapeLayer.CapRound,
                LineWidth = (float)DefaultStroke,
                StrokeEnd = 0,
                AnchorPoint = new CGPoint(0.5, 0.5)
            };

            view.Layer.AddSublayer(_trackLayer);
            view.Layer.AddSublayer(_progressLayer);

            return view;
        }

        protected override void ConnectHandler(UIView platformView)
        {
            base.ConnectHandler(platformView);

            if (VirtualView != null)
                VirtualView.SizeChanged += VirtualView_SizeChanged;

            platformView.SetNeedsLayout();
            platformView.LayoutIfNeeded();

            UpdateLayers();
        }

        protected override void DisconnectHandler(UIView platformView)
        {
            base.DisconnectHandler(platformView);

            if (VirtualView != null)
                VirtualView.SizeChanged -= VirtualView_SizeChanged;

            StopIndeterminateAnimation();
        }

        private void VirtualView_SizeChanged(object sender, System.EventArgs e)
        {
            UpdateLayers();
        }

        void UpdateLayers()
        {
            if (VirtualView is null || PlatformView is null) return;

            var width = PlatformView.Bounds.Width > 0 ? PlatformView.Bounds.Width : (VirtualView.WidthRequest > 0 ? VirtualView.WidthRequest : DefaultSize);
            var height = PlatformView.Bounds.Height > 0 ? PlatformView.Bounds.Height : (VirtualView.HeightRequest > 0 ? VirtualView.HeightRequest : DefaultSize);

            if (width <= 0 || height <= 0) return;

            var layerFrame = new CGRect(0, 0, width, height);
            _trackLayer.Frame = layerFrame;
            _progressLayer.Frame = layerFrame;

            var center = new CGPoint(layerFrame.Width / 2.0, layerFrame.Height / 2.0);
            var stroke = VirtualView.StrokeThickness > 0 ? VirtualView.StrokeThickness : DefaultStroke;
            var radius = Math.Max(0, Math.Min(layerFrame.Width, layerFrame.Height) / 2.0 - (stroke / 2.0));

            var startAngle = -Math.PI / 2.0;
            var endAngle = 3.0 * Math.PI / 2.0;

            var circlePath = UIBezierPath.FromArc(center, (NFloat)radius, (NFloat)startAngle, (NFloat)endAngle, true);

            _trackLayer.Path = circlePath.CGPath;
            _trackLayer.LineWidth = (float)stroke;
            _trackLayer.StrokeColor = VirtualView.TrackColor?.ToPlatform()?.CGColor ?? UIColor.LightGray.CGColor;

            _progressLayer.Path = circlePath.CGPath;
            _progressLayer.LineWidth = (float)stroke;
            _progressLayer.StrokeColor = VirtualView.ProgressColor?.ToPlatform()?.CGColor ?? UIColor.SystemPink.CGColor;

            _progressLayer.RemoveAnimation("rotationAnimation");

            if (VirtualView.IsIndeterminate)
            {
                SetProgress(0.25, animate: false);
                StartIndeterminateAnimation();
            }
            else
            {
                StopIndeterminateAnimation();
                SetProgress(VirtualView.Progress, animate: false);
            }
        }

        void SetProgress(double progress, bool animate = true)
        {
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

        void StartIndeterminateAnimation()
        {
            if (_progressLayer.AnimationForKey("rotationAnimation") != null) return;

            var rotation = CABasicAnimation.FromKeyPath("transform.rotation.z");
            rotation.From = NSNumber.FromDouble(0);
            rotation.To = NSNumber.FromDouble(2.0 * Math.PI);
            rotation.Duration = 1.0;
            rotation.RepeatCount = float.MaxValue;
            rotation.RemovedOnCompletion = false;

            _progressLayer.AddAnimation(rotation, "rotationAnimation");
        }

        void StopIndeterminateAnimation()
        {
            _progressLayer.RemoveAnimation("rotationAnimation");
        }

        public static void MapProgress(ProgressRingHandler handler, ProgressRing view)
        {
            if (handler == null || view == null) return;
            if (!view.IsIndeterminate)
                handler.SetProgress(view.Progress, animate: true);
        }

        public static void MapProgressColor(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateLayers();
        public static void MapTrackColor(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateLayers();
        public static void MapStrokeThickness(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateLayers();
        public static void MapIsIndeterminate(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateLayers();
        public static void MapSize(ProgressRingHandler handler, ProgressRing view) => handler?.UpdateLayers();
    }
}
