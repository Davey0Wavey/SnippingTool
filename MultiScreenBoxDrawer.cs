using System;
using System.Drawing;
using System.Windows.Forms;

public class MultiScreenBoxDrawer : Form
{
    private string snapStatus = "ready";
    private Point startPoint, currentPoint;
    private BoxManager boxManager;
    private Bitmap virtualDesktopScreenshot;
    private string cornerTag;

    public MultiScreenBoxDrawer()
    {
        this.DoubleBuffered = true;
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.BackColor = Color.Black;
        this.Cursor = Cursors.Cross;

        // Initialize helpers
        virtualDesktopScreenshot = ScreenshotHelper.CaptureVirtualDesktop();
        boxManager = new BoxManager();

        // Event handlers
        this.MouseDown += OnMouseDown;
        this.MouseMove += OnMouseMove;
        this.MouseUp += OnMouseUp;
        this.Paint += OnPaint;
        this.cornerTag = "dead-duck";
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (snapStatus == "ready" && e.Button == MouseButtons.Left)
        {
            snapStatus = "drawing";
            Console.WriteLine("OnMouseDown: " + snapStatus);
            startPoint = Cursor.Position;
        }
        else if (snapStatus == "drawn" && e.Button == MouseButtons.Left && boxManager.IsNearCorner(Cursor.Position))
        {
            snapStatus = "adjusting";
            Console.WriteLine("OnMouseDown: " + snapStatus);
            cornerTag = boxManager.GetCorner(Cursor.Position);
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (snapStatus == "drawing")
        {
            currentPoint = Cursor.Position;
            boxManager.UpdateMainBox(startPoint, currentPoint);
            Invalidate();
        }
        if (snapStatus == "drawn" || snapStatus == "adjusting")
        {
            Cursor = boxManager.IsNearCorner(Cursor.Position) ? Cursors.SizeAll : Cursors.Default;
        }
        if (snapStatus == "adjusting")
        {
            startPoint = Cursor.Position;
            boxManager.AdjustCorner(cornerTag, Cursor.Position);
            Invalidate();
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if ((snapStatus == "drawing" || snapStatus == "adjusting") && e.Button == MouseButtons.Left)
        {
            snapStatus = "drawn";
            Console.WriteLine("OnMouseUp: " + snapStatus);
        }
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.DrawImage(virtualDesktopScreenshot, ScreenshotHelper.GetVirtualScreenBounds());

        // Apply an 85% transparent grey overlay
        if (snapStatus == "ready")
        {
            using (Brush semiTransparentBrush = new SolidBrush(Color.FromArgb(128, Color.Gray))) // 217 = 85% opacity
            {
                g.FillRectangle(semiTransparentBrush, ScreenshotHelper.GetVirtualScreenBounds());
            }
        }
        else if (snapStatus == "drawing" || snapStatus == "adjusting")
        {
            boxManager.DrawBoxes(g);
        }
        
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        virtualDesktopScreenshot?.Dispose();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            switch (snapStatus)
            {
                case "ready":
                    // Exit the application
                    Application.Exit();
                    Console.WriteLine(snapStatus);
                    break;

                case "drawing":
                    snapStatus = "ready";
                    Console.WriteLine(snapStatus);
                    Invalidate(); // Redraw the form to clear any visuals
                    break;

                case "drawn":
                    snapStatus = "ready";
                    Console.WriteLine(snapStatus);
                    Invalidate(); // Redraw the form to clear any visuals
                    break;

                case "adjusting":
                    // Cancel adjustments, reset status to "ready"
                    snapStatus = "drawn";
                    Console.WriteLine(snapStatus);
                    //boxManager.Reset(); // Reset the program to its initialized state
                    Invalidate(); // Redraw to remove adjustments
                    break;

                default:
                    // Handle unexpected status (if needed)
                    break;
            }
            return true; // Indicate that the key press was handled
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

}
