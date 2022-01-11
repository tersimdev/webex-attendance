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
    public partial class InfoForm : Form
    {
        public void SetDebugText(string text)
        {
            debugLabel.Text = text;
        }

        public InfoForm()
        {
            InitializeComponent();
        }
    }
}
