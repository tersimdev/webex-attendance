using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebExAttendance_Form
{
    public partial class Frame : Form
    {
        public Frame()
        {
            InitializeComponent();
            SetStyle(ControlStyles  .SupportsTransparentBackColor, true);
            this.Opacity = 0.5;
        }

        private void Frame_Load(object sender, EventArgs e)
        {

        }
    }
}
