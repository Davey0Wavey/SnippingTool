using System;
using System.Drawing;

public class BoxManager
{
    private const int CornerThreshold = 10;
    public Rectangle MainBox { get; private set; }
    public Rectangle TopBox { get; private set; }
    public Rectangle LeftBox { get; private set; }
    public Rectangle RightBox { get; private set; }
    public Rectangle BottomBox { get; private set; }

    public void UpdateMainBox(Point start, Point current)
    {
        MainBox = GetRectangleFromPoints(start, current);
        UpdateSurroundingBoxes();
    }

    public bool IsNearCorner(Point cursorPos)
    {
        return GetCorner(cursorPos) != "dead-duck";
    }

    public void TryAdjustCorner(Point cursorPos)
    {
        string corner = GetCorner(cursorPos);
        if (corner != "dead-duck")
        {
            AdjustCorner(corner, cursorPos);
        }
    }

    public void DrawBoxes(Graphics g)
    {
        using (Brush greyBrush = new SolidBrush(Color.FromArgb(128, Color.Gray)))
        {
            g.FillRectangle(greyBrush, TopBox);
            g.FillRectangle(greyBrush, LeftBox);
            g.FillRectangle(greyBrush, RightBox);
            g.FillRectangle(greyBrush, BottomBox);
        }
        using (Pen borderPen = new Pen(Color.Red, 2))
        {
            g.DrawRectangle(borderPen, MainBox);
        }
    }

    private void UpdateSurroundingBoxes()
    {
        Rectangle virtualBounds = ScreenshotHelper.GetVirtualScreenBounds();

        TopBox = new Rectangle(virtualBounds.X, virtualBounds.Y, virtualBounds.Width, MainBox.Top - virtualBounds.Y);
        Console.WriteLine("TopBox X: " + virtualBounds.X + " Y: " + virtualBounds.Y + " X Width: " + virtualBounds.Width + " X Height: " + (MainBox.Top - virtualBounds.Y));

        LeftBox = new Rectangle(virtualBounds.X, MainBox.Top, MainBox.Left - virtualBounds.X, MainBox.Height);
        Console.WriteLine("LeftBox X: " + virtualBounds.X + " Y: " + MainBox.Top + " X Width: " + (MainBox.Left - virtualBounds.X) + " X Height: " + MainBox.Height);

        RightBox = new Rectangle(MainBox.Right, MainBox.Top, virtualBounds.Right - MainBox.Right, MainBox.Height);
        Console.WriteLine("RightBox X: " + MainBox.Right + " Y: " + MainBox.Top + " X Width: " + (virtualBounds.Right - MainBox.Right) + " X Height: " + MainBox.Height);

        BottomBox = new Rectangle(virtualBounds.X, MainBox.Bottom, virtualBounds.Width, virtualBounds.Bottom - MainBox.Bottom);
        Console.WriteLine("BottomBox X: " + virtualBounds.X + " Y: " + MainBox.Bottom + " X Width: " + virtualBounds.Width + " X Height: " + (virtualBounds.Bottom - MainBox.Bottom));
    }

    public string GetCorner(Point cursorPos)
    {
        if (IsWithinThreshold(cursorPos, MainBox.Location)) return "top-left";
        if (IsWithinThreshold(cursorPos, new Point(MainBox.Right, MainBox.Top))) return "top-right";
        if (IsWithinThreshold(cursorPos, new Point(MainBox.Left, MainBox.Bottom))) return "bottom-left";
        if (IsWithinThreshold(cursorPos, new Point(MainBox.Right, MainBox.Bottom))) return "bottom-right";
        return "dead-duck";
    }

    public void AdjustCorner(string corner, Point newPos)
    {
        switch (corner)
        {
            case "top-left":
                MainBox = new Rectangle(newPos.X, newPos.Y, MainBox.Right - newPos.X, MainBox.Bottom - newPos.Y);
                break;
            case "top-right":
                MainBox = new Rectangle(MainBox.Left, newPos.Y, newPos.X - MainBox.Left, MainBox.Bottom - newPos.Y);
                break;
            case "bottom-left":
                MainBox = new Rectangle(newPos.X, MainBox.Top, MainBox.Right - newPos.X, newPos.Y - MainBox.Top);
                break;
            case "bottom-right":
                MainBox = new Rectangle(MainBox.Left, MainBox.Top, newPos.X - MainBox.Left, newPos.Y - MainBox.Top);
                break;
        }
        UpdateSurroundingBoxes();
    }

    private bool IsWithinThreshold(Point p1, Point p2)
    {
        return Math.Abs(p1.X - p2.X) <= CornerThreshold && Math.Abs(p1.Y - p2.Y) <= CornerThreshold;
    }

    private Rectangle GetRectangleFromPoints(Point p1, Point p2)
    {
        return new Rectangle(
            Math.Min(p1.X, p2.X),
            Math.Min(p1.Y, p2.Y),
            Math.Abs(p1.X - p2.X),
            Math.Abs(p1.Y - p2.Y));
    }
    public void Reset()
    {
        // Reset all state variables to initial values
        
        MainBox = Rectangle.Empty;
        TopBox = Rectangle.Empty;
        LeftBox = Rectangle.Empty;
        RightBox = Rectangle.Empty;
        BottomBox = Rectangle.Empty;
    }
    public void UpdateAdjacentBoxes(Point p1, string corner)
    {

    }
}
