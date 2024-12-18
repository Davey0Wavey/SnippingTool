using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

public static class ScreenshotHelper
{
    public static Bitmap CaptureVirtualDesktop()
    {
        Rectangle virtualBounds = GetVirtualScreenBounds();
        Bitmap screenshot = new Bitmap(virtualBounds.Width, virtualBounds.Height, PixelFormat.Format32bppArgb);

        using (Graphics g = Graphics.FromImage(screenshot))
        {
            foreach (var screen in Screen.AllScreens)
            {
                Rectangle bounds = screen.Bounds;
                g.CopyFromScreen(bounds.Location, new Point(bounds.X - virtualBounds.X, bounds.Y - virtualBounds.Y), bounds.Size);
            }
        }

        return screenshot;
    }

    public static Rectangle GetVirtualScreenBounds()
    {
        int minX = Screen.AllScreens.Min(s => s.Bounds.X);
        int minY = Screen.AllScreens.Min(s => s.Bounds.Y);
        int maxX = Screen.AllScreens.Max(s => s.Bounds.Right);
        int maxY = Screen.AllScreens.Max(s => s.Bounds.Bottom);

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }
}
