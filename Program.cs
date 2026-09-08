using Avalonia;
using System;

namespace EService;

// This is the actual entry point of the whole application (like Java's
// "public static void main" in Main.java). All it does is hand control
// over to Avalonia, which then builds and shows the MainWindow defined
// in App.axaml.cs.
internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
