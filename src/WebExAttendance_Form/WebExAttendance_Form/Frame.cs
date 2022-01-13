using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using WindowsInput;

namespace WebExAttendance_Form
{
    public partial class Frame : Form
    {
        private InputSimulator inputSimulator;
        public Frame()
        {
            InitializeComponent();
            SetStyle(ControlStyles  .SupportsTransparentBackColor, true);
            this.Opacity = 0.5;
            this.TopMost = true;
            inputSimulator = new InputSimulator();
        }

        private void Frame_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        public void Click()
        {
            var centerOfWindow = new System.Drawing.Point();
            centerOfWindow.X = this.DesktopLocation.X + (this.DesktopBounds.Width / 2);
            centerOfWindow.Y = this.DesktopLocation.Y + (this.DesktopBounds.Height / 2);

            Cursor.Position = centerOfWindow;

            inputSimulator.Mouse.Sleep(50);
            inputSimulator.Mouse.LeftButtonClick();
        }

        public void Scroll(int amtClicks, int dir = -1)
        {
            for (int i = 0; i < amtClicks; ++i)
            {
                inputSimulator.Mouse.VerticalScroll(dir);
                inputSimulator.Mouse.Sleep(50);
            }
        }
    }
}
