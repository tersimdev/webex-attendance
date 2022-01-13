using System;
using System.Drawing;
using System.Windows.Forms;

namespace WebExAttendance_Form
{
    public partial class MainForm : Form
    {
        //State
        private Frame frame = null;
        private InfoForm infoForm = null;
        private Image image = null;
        private string imageFilePath = "";

        private Inference inference;

        #region Init
        public MainForm()
        {
            InitializeComponent();
            inference = new Inference();
        }
         
        private void MainForm_Load(object sender, EventArgs e)
        {
            ResetState();
        }
        #endregion

        #region StateHelper
        private void UpdateImageState(Image img, string statusText = "")
        {
            image = img;
            picturePreview.Image = image;
            pictureCaption.Text = statusText;


            if (img == null)
            {
                btnReset.Hide();
                btnDetect.Hide();
            }
            else
            {
                btnReset.Show();
                btnDetect.Show();
            }
        }

        private void ResetState()
        {
            if (frame != null)
                frame.Close();
            if (infoForm != null)
                infoForm.Close();

            frame = new Frame();
            frame.Hide();

            infoForm = new InfoForm();
            infoForm.Hide();            

            frame.FormClosing += FrameFormClosing;
            infoForm.FormClosing += InfoFormClosing;

            btnSelectImageScreen.Hide();
            btnHideScreenshotWindow.Hide();
            clipboardNoImgLabel.Hide();

            UpdateImageState(null);
        }

        private void InfoFormClosing(object sender, FormClosingEventArgs args)
        {
            args.Cancel = true;
            infoForm.Hide();
        }

        private void FrameFormClosing(object sender, FormClosingEventArgs args)
        {
            args.Cancel = true;
            frame.Hide();
            btnSelectImageScreen.Hide();
            btnHideScreenshotWindow.Hide();
        }

        #endregion

        #region ButtonCallbacks
        private void btnShowScreenshotWindow_Click(object sender, EventArgs e)
        {
            frame.Show();
            btnSelectImageScreen.Show();
            btnHideScreenshotWindow.Show();
        }

        private void btnSelectImageFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.png; *.jpg; *.jpeg; *.gif; *.bmp)|*.png; *.jpg; *.jpeg; *.gif; *.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                imageFilePath = open.FileName;
                UpdateImageState(new Bitmap(imageFilePath), "Filepath: " + imageFilePath);
            }
            else
            {
                //hide UI                
                UpdateImageState(null);
            }
        }

        private void btnSelectImageClipboard_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                Image clipboardImg = Clipboard.GetImage();
                if (clipboardImg != null)
                {
                    UpdateImageState(clipboardImg, "Image pasted from clipboard.");
                    clipboardNoImgLabel.Hide();
                }
                else
                    clipboardNoImgLabel.Show();
            }
            else
                clipboardNoImgLabel.Show();
        }

        private void btnSelectImageScreen_Click(object sender, EventArgs e)
        {
            frame.Hide();

            //take screenshot of desktop at specified rect of form             
            Rectangle rect = frame.DesktopBounds;
            image = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(image);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, frame.Size, CopyPixelOperation.SourceCopy);

            UpdateImageState(image, "Screenshot taken.");

            frame.Show();
        }

        private void btnHideScreenshotWindow_Click(object sender, EventArgs e)
        {
            frame.Hide();
            btnSelectImageScreen.Hide();
            btnHideScreenshotWindow.Hide();
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            frame.Hide();
            btnSelectImageScreen.Hide();
            btnHideScreenshotWindow.Hide();
            var results = inference.DoAnalysis(image);
            if (results != null)
            {
                infoForm.ProcessWords(results);
                infoForm.Show();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetState();
        }

        #endregion
    }
}
