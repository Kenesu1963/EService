using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace EService.Services;

/// <summary>
/// Small helper to load one of the bundled dish icons (from Assets/Dishes)
/// as a Bitmap that an Avalonia &lt;Image&gt; control can display. Centralizing
/// this in one place means if an icon is missing we fail gracefully (no
/// icon shown) instead of crashing the whole screen.
/// </summary>
public static class AssetImageLoader
{
    public static Bitmap? TryLoad(string relativePath)
    {
        try
        {
            var uri = new System.Uri($"avares://EService/{relativePath}");
            using var stream = AssetLoader.Open(uri);
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }
}
