using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ENG.Test.Resources;

internal static class ImageManager
{
    /// <summary>
    /// Get the icon from the build binaries.
    /// </summary>
    public static BitmapImage GetIcon(string name)
    {
        var iconResourceLocation = typeof(ImageManager).Namespace + "Resources.Icons." + name;
        var uri = new Uri("pack://application:,,,/ENG.Test;component/Resources/Icons/" + name);
        var resourceStream = Application.GetResourceStream(uri)?.Stream;
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(iconResourceLocation);

        var image = new BitmapImage();

        image.BeginInit();
        image.StreamSource = resourceStream;
        image.EndInit();

        return image;
    }
}