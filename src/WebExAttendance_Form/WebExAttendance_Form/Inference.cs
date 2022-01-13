using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace WebExAttendance_Form
{
    class Inference
    {
        private TextDetectionModel_DB model_textDet = null;
        private TextRecognitionModel model_textRec = null;
        private Size textDetectInputSize = new Size(736, 960);
        private Size textRecInputSize = new Size(100, 32);
        private const float binaryThreshold = 0.1f;
        private const float polygonThreshold = 0.5f;
        private const int maxCandidates = 200;
        private string[] vocab = null;

        public Inference()
        {
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

        public List<string> DoAnalysis(Image img)
        {
            Bitmap inputBmp = ResizeAndPadImg(new Bitmap(img), textDetectInputSize, Color.Black);
            Bitmap inputBmpCopy = new Bitmap(inputBmp); //make a copy to preserve input
            //var textDetectInput = inputBmpCopy.ToImage<Bgr, byte>().ThresholdBinaryInv(new Bgr(150,150,170), new Bgr(Color.White));
            var textDetectInput = inputBmpCopy.ToImage<Bgr, byte>().ThresholdBinaryInv(new Bgr(150,150,150), new Bgr(Color.White));

            //CvInvoke.Imshow("Input img resized", textDetectInput); //debug

            //////// TEXT DETECTION
            //prepare input array
            IInputArray textDetInputArr = textDetectInput;
            //prepare text detect output
            VectorOfVectorOfPoint detections = new VectorOfVectorOfPoint();
            VectorOfFloat confidences = new VectorOfFloat();
            //detect text
            model_textDet.Detect(textDetInputArr, detections, confidences);

            if (detections.Size >= 0)
            {
                //crop images into a list
                var croppedImgs = new List<Image<Gray, byte>>(confidences.Size);
                for (int i = 0; i < detections.Size; ++i)
                {
                    Rectangle cropArea = VectorOfPointsToRect(detections[i], inputBmp.Width, inputBmp.Height, -1);
                    Bitmap croppedBmp = CropImg(inputBmp, cropArea);
                    croppedBmp = ResizeAndPadImg(croppedBmp, textRecInputSize, Color.Black, 0.3);
                    var croppedImg = croppedBmp.ToImage<Gray, byte>();
                    croppedImg = croppedImg.ThresholdBinaryInv(new Gray(140), new Gray(255));
                    var element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
                    CvInvoke.Erode(croppedImg, croppedImg, element, new Point(-1, -1), 1, BorderType.Constant, new MCvScalar(255, 255, 255));
                    croppedImgs.Add(croppedImg);
                }

#if FALSE  // VISUALIZE TEXT DETECT RESULTS
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
                    //for (int i = 0; i < Math.Min(5, detections.Size); ++i)
                    //{
                    //    var croppedImg = croppedImgs[i];
                    //    CvInvoke.Resize(croppedImg, croppedImg, new Size(croppedImg.Width * 2, croppedImg.Height * 2), 0, 0, Inter.Cubic);
                    //    CvInvoke.Imshow("Detection Results Cropped " + i.ToString(), croppedImg);
                    //}
                }
#endif

                //////// TEXT RECOGNITION               
                //recognise text
                List<string> recognisedText = new List<string>(croppedImgs.Count);
                foreach (var textRecInput in croppedImgs)
                {
                    string res = model_textRec.Recognize(textRecInput);
                    recognisedText.Add(res);
                }

                return recognisedText;
            }
            return null;
        }


        #region Helpers
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
    }
}
