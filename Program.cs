using System;
using System.Windows.Forms;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Initialize the main form
        var boxDrawerForm = new MultiScreenBoxDrawer();
        Application.Run(boxDrawerForm);
    }
}
