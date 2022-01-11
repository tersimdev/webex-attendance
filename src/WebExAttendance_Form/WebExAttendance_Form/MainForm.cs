using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
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
        private string[] vocab = null;

        //CV
        private TextDetectionModel_DB model_textDet = null;
        private TextRecognitionModel model_textRec = null;
        private Size textDetectInputSize = new Size(736, 736);
        private Size textRecInputSize = new Size(100, 32);
        private const float binaryThreshold = 0.1f;
        private const float polygonThreshold = 0.5f;
        private const int maxCandidates = 200;

        #region Init
        public MainForm()
        {
            InitializeComponent();
        }
         
        private void MainForm_Load(object sender, EventArgs e)
        {
            ResetState();
            //init CV stuff
            CvInvoke.Init();
            //load in models
            model_textDet = new TextDetectionModel_DB("models/DB_IC15_resnet18.onnx");
            model_textDet.UnclipRatio = 2.0f;
            model_textDet.BinaryThreshold = binaryThreshold;
            model_textDet.MaxCandidates = maxCandidates;
            model_textDet.PolygonThreshold = polygonThreshold;
            model_textDet.SetInputSize(textDetectInputSize);
            model_textDet.SetInputScale(1.0 / 255);
            model_textDet.SetInputMean(new MCvScalar(122.67891434, 116.66876762, 104.00698793));
            model_textRec = new TextRecognitionModel("models//CRNN_VGG_BiLSTM_CTC.onnx");
            model_textRec.DecodeType = "CTC-greedy";
            vocab = LoadVocab("models/alphabet_36.txt");
            model_textRec.Vocabulary = vocab;
            model_textRec.SetInputSize(textRecInputSize);
            model_textRec.SetInputScale(1.0 / 127.5);
            model_textRec.SetInputMean(new MCvScalar(127.5, 127.5, 127.5));
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

        #region OpenCV
        private void DoAnalysis(Image img)
        {
            Bitmap inputBmp = ResizeAndPadImg(new Bitmap(img), textDetectInputSize, Color.Black);
            Bitmap inputBmpCopy = new Bitmap(inputBmp); //make a copy to preserve input
            var textDetectInput = inputBmpCopy.ToImage<Bgr, byte>();
            //CvInvoke.Imshow("Input img resized", textDetectInput); //debug

            //////// TEXT DETECTION
            //prepare input array
            IInputArray textDetInputArr = textDetectInput;
            //prepare text detect output
            VectorOfVectorOfPoint detections = new VectorOfVectorOfPoint();
            VectorOfFloat confidences = new VectorOfFloat();
            //detect text
            model_textDet.Detect(textDetInputArr, detections, confidences);

            string results = "";

            if (confidences.Size < 0)
            {
                results = "No text detected";
            }
            else
            {
                //crop images into a list
                var croppedImgs = new List<Image<Gray, byte>>(confidences.Size);
                for (int i = 0; i < detections.Size; ++i)
                {
                    Rectangle cropArea = VectorOfPointsToRect(detections[i], inputBmp.Width, inputBmp.Height, -1);
                    Bitmap croppedBmp = CropImg(inputBmp, cropArea);
                    croppedBmp = ResizeAndPadImg(croppedBmp, textRecInputSize, Color.Black, 0.3);
                    croppedImgs.Add(croppedBmp.ToImage<Gray, byte>().ThresholdBinaryInv(new Gray(170), new Gray(255)));
                }

#if TRUE  // VISUALIZE TEXT DETECT RESULTS
                {
                    IInputOutputArray testImg = textDetectInput;
                    for (int i = 0; i < detections.Size; ++i)
                    {
                        //results += i.ToString() + ": " + confidences[i].ToString() + "\n";
                        CvInvoke.Line(testImg, detections[i][0], detections[i][1], new MCvScalar(255, 0, 0), 1);
                        CvInvoke.Line(testImg, detections[i][1], detections[i][2], new MCvScalar(255, 0, 0), 1);
                        CvInvoke.Line(testImg, detections[i][2], detections[i][3], new MCvScalar(255, 0, 0), 1);
                        CvInvoke.Line(testImg, detections[i][3], detections[i][0], new MCvScalar(255, 0, 0), 1);
                    }
                    CvInvoke.Resize(testImg, testImg, new Size(800, 800), 0, 0, Inter.Cubic);
                    CvInvoke.Imshow("Detection Results", testImg);
                    for (int i = 0; i < Math.Min(3, confidences.Size); ++i)
                    {
                        var croppedImg = croppedImgs[i];
                        CvInvoke.Resize(croppedImg, croppedImg, new Size(croppedImg.Width * 2, croppedImg.Height * 2), 0, 0, Inter.Cubic);
                        CvInvoke.Imshow("Detection Results Cropped " + i.ToString(), croppedImg);
                    }
                }
#endif

                //////// TEXT RECOGNITION               
                //recognise text
                List<string> recognisedText = new List<string>(croppedImgs.Count);
                foreach (var textRecInput in croppedImgs)
                {
                    string res = model_textRec.Recognize(textRecInput);   
                    recognisedText.Add(res);
                    Console.WriteLine(res);
                }

                //let infoForm process results
                infoForm.ProcessWords(recognisedText);                
            }            
            infoForm.Show();
        }
        private Bitmap ResizeAndPadImg(Bitmap bmp, Size targetSize, Color padColor, double aspectBias = 1)
        {
            Bitmap ret = new Bitmap(targetSize.Width, targetSize.Height);
            double aspect = (double)bmp.Width / (double)bmp.Height;
            double biasedAspect = aspect * aspectBias;
            using (Graphics graphics = Graphics.FromImage(ret))
            {
                graphics.Clear(padColor);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;

                if (biasedAspect > 1) //horizontal
                {
                    int h = (int)Math.Floor(((double)ret.Width) / aspect);
                    graphics.DrawImage(bmp, 0, 0, ret.Width, h);
                }
                else if (biasedAspect < 1) //vertical
                {
                    int w = (int)Math.Floor(((double)ret.Height) * aspect);

                    graphics.DrawImage(bmp, 0, 0, w, ret.Height);
                }
                else
                {
                    graphics.DrawImage(bmp, 0, 0, ret.Width, ret.Height);
                }
            }

            return ret;
        }

        private Bitmap CropImg(Bitmap bmp, Rectangle cropArea)
        {
            Bitmap retBmp = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics g = Graphics.FromImage(retBmp))
                g.DrawImage(bmp, -cropArea.X, -cropArea.Y);

            return retBmp;
        }

        private Rectangle VectorOfPointsToRect(VectorOfPoint vec, int width, int height, int expandAmt)
        {
            List<int> pointsX = new List<int> { vec[0].X, vec[1].X, vec[2].X, vec[3].X };
            List<int> pointsY = new List<int> { vec[0].Y, vec[1].Y, vec[2].Y, vec[3].Y };

            pointsX.Sort();
            pointsY.Sort();

            pointsX[0] = Math.Max(0, pointsX[0] - expandAmt);
            pointsY[0] = Math.Max(0, pointsY[0] - expandAmt);

            int rw = Math.Min(width - pointsX[0], pointsX[3] - pointsX[0] + expandAmt * 2);
            int rh = Math.Min(height - pointsY[0], pointsY[3] - pointsY[0] + expandAmt * 2);

            return new Rectangle(pointsX[0], pointsY[0], rw, rh);
        }

        private string[] LoadVocab(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i].Trim();
            }
            return lines;
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
            //hide the form so that it is not captured
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
            DoAnalysis(image);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetState();
        }

        #endregion
    }
}
