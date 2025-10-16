namespace Maui.ProgressRing;

/// <summary>
/// Represents a cross-platform native progress ring for .NET MAUI.
/// Supports both determinate and indeterminate modes with customizable
/// stroke thickness and colors.
/// </summary>
public class ProgressRing : View
{
    /// <summary>
    /// Identifies the <see cref="Progress"/> bindable property.
    /// </summary>
    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(ProgressRing), 0.0,
            propertyChanged: (b, o, n) => ((ProgressRing)b).InvalidateMeasure());

    /// <summary>
    /// Gets or sets the progress value, ranging from 0.0 to 1.0.
    /// Only applies when <see cref="IsIndeterminate"/> is <c>false</c>.
    /// </summary>
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, Math.Clamp(value, 0.0, 1.0));
    }

    /// <summary>
    /// Identifies the <see cref="IsIndeterminate"/> bindable property.
    /// </summary>
    public static readonly BindableProperty IsIndeterminateProperty =
        BindableProperty.Create(nameof(IsIndeterminate), typeof(bool), typeof(ProgressRing), false);

    /// <summary>
    /// Gets or sets a value indicating whether the progress ring
    /// shows an indeterminate spinning animation.
    /// </summary>
    public bool IsIndeterminate
    {
        get => (bool)GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="StrokeThickness"/> bindable property.
    /// </summary>
    public static readonly BindableProperty StrokeThicknessProperty =
        BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(ProgressRing), 4.0);

    /// <summary>
    /// Gets or sets the width of the circular stroke.
    /// </summary>
    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TrackColor"/> bindable property.
    /// </summary>
    public static readonly BindableProperty TrackColorProperty =
        BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(ProgressRing), Colors.Transparent);

    /// <summary>
    /// Gets or sets the color of the background track of the progress ring.
    /// </summary>
    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ProgressColor"/> bindable property.
    /// </summary>
    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(ProgressRing), Colors.DodgerBlue);

    /// <summary>
    /// Gets or sets the color of the animated progress arc.
    /// </summary>
    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }
}
