using System.Drawing;
using System.Windows.Forms;

public class SnipperForm : Form
{
    private Bitmap screenshot;

    public SnipperForm(Bitmap screenshot)
    {
        this.screenshot = screenshot;
        InitializeForm();
    }

    private void InitializeForm()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.Manual;
        this.Bounds = Screen.FromControl(this).Bounds;
        this.BackgroundImage = screenshot;
        this.ShowInTaskbar = false;
    }
}
