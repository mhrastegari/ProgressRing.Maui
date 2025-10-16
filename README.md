# ProgressRing.Maui

A cross-platform native .NET MAUI ProgressRing (Circular Progress) control for Android, iOS, macOS, and Windows.\
Supports determinate and indeterminate modes with customizable colors and stroke thickness.

## ✨ Features

- ✅ Determinate and indeterminate modes
- ✅ Customizable progress and track colors
- ✅ Adjustable stroke thickness
- ✅ Consistent native appearance across all platforms

## 📦 Installation

Install via NuGet:

```bash
dotnet add package ProgressRing.Maui
```

## ⚙️ Setup

Add the handler registration in your `MauiProgram.cs` file:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder
        .UseMauiApp<App>()
        .UseProgressRing()
        ...
}
```

## 💡 Usage Example

In your XAML page:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:mhr="http://mhrastegari.com/progressring"
             x:Class="MyMauiApp.MainPage">
    <StackLayout Spacing="24" Padding="24">
        <!-- Determinate ProgressRing -->
        <mhr:ProgressRing Progress="0.6"
                          StrokeThickness="6"
                          ProgressColor="#512BD4"
                          TrackColor="#C8C8C8"
                          WidthRequest="85"
                          HeightRequest="85" />

        <!-- Indeterminate ProgressRing -->
        <mhr:ProgressRing IsIndeterminate="True"
                          StrokeThickness="6"
                          ProgressColor="DodgerBlue"
                          WidthRequest="85"
                          HeightRequest="85" />
    </StackLayout>
</ContentPage>
```

Or in code-behind:

```csharp
using ProgressRing.Maui;

var ring = new ProgressRing
{
    IsIndeterminate = true,
    StrokeThickness = 6,
    ProgressColor = Colors.DodgerBlue,
    WidthRequest = 85,
    HeightRequest = 85
};
```

## 🧬 Supported Platforms

| Platform | Supported | Notes                                      |
| -------- | --------- | ------------------------------------------ |
| Android  | ✅        | Native `CircularProgressIndicator`         |
| Windows  | ✅        | Native `ProgressRing` control              |
| iOS      | ✅        | Uses a custom `CAShapeLayer` based drawing |
| macOS    | ✅        | Shared with iOS implementation             |

## 📝 License

This project is licensed under the [MIT License](./LICENSE).
