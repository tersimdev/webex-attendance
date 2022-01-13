using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WebExAttendance_Form
{
    public partial class Frame : Form
    {
        public Frame()
        {
            InitializeComponent();
            SetStyle(ControlStyles  .SupportsTransparentBackColor, true);
            this.Opacity = 0.5;
            this.TopMost = true;
        }

        #region Windows API
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
            [FieldOffset(0)] public HardwareInput hi;
        }
        public struct Input
        {
            public int type;
            public InputUnion u;
        }
        [Flags]
        public enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }
        [Flags]
        public enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008
        }
        [Flags]
        public enum MouseEventF
        {
            Absolute = 0x8000,
            HWheel = 0x01000,
            Move = 0x0001,
            MoveNoCoalesce = 0x2000,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            VirtualDesk = 0x4000,
            Wheel = 0x0800,
            XDown = 0x0080,
            XUp = 0x0100
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        private const int WHEEL_DELTA = 120;

        #endregion


        private void Frame_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        public void Scroll(int amtClicks, int dir = -1)
        {
            //bring mouse to window center                
            SetCursorPos(this.DesktopLocation.X + this.DesktopBounds.Width / 2, this.DesktopLocation.Y + this.DesktopBounds.Height / 2);

            Input clickDown = new Input();
            clickDown.type = (int)InputType.Mouse;
            clickDown.u = new InputUnion
            {
                mi = new MouseInput
                {
                    dwFlags = (uint)(MouseEventF.LeftDown),
                    dwExtraInfo = GetMessageExtraInfo()
                }
            };
            Input clickUp= new Input();
            clickUp.type = (int)InputType.Mouse;
            clickUp.u = new InputUnion
            {
                mi = new MouseInput
                {
                    dwFlags = (uint)(MouseEventF.LeftUp),
                    dwExtraInfo = GetMessageExtraInfo()
                }
            };
            Input scroll = new Input();
            scroll.type = (int)InputType.Mouse;
            scroll.u = new InputUnion
            {
                mi = new MouseInput
                {
                    mouseData = (uint)(WHEEL_DELTA * amtClicks * dir),
                    dwFlags = (uint)(MouseEventF.Wheel),
                    dwExtraInfo = GetMessageExtraInfo()
                }
            };

            Input[] inputs = new Input[] { clickDown, clickUp, scroll };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
